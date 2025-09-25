# WorkshopCode_ER_MVP
**Version:** v2
**Date:** 2025-09-24  
**Status:** In Review  

```mermaid
---
config:
  layout: elk
  look: classic
  theme: redux-dark-color
---
erDiagram
	direction TB
	IDENTITY_USER {
		uuid Id PK "source of truth"  
		string UserName  ""  
		string Email  ""  
		string DisplayName  ""  
		string Telegram  ""  
		string Role  "MVP: string role"  
	}
	StudentTopUp {
		uuid Id PK ""  
		uuid StudentId FK "→ IDENTITY_USER.Id (Student), NN, on delete restrict"  
		numeric HoursAdded  "NN"  
		string Comment  ""  
		uuid AdminId FK "→ IDENTITY_USER.Id (Admin), NN, on delete restrict"  
		datetime CreatedAt  ""  
	}
	StudentBalance {
		uuid StudentId PK,FK "→ IDENTITY_USER.Id (Student), on delete restrict"  
		numeric HoursAdded  "NN"  
		numeric HoursSpent  "NN"  
		numeric HoursRemaining  "NN"  
		datetime CalculatedAt  "NN"  
	}
	Course {
		uuid Id PK ""  
		string Title  "NN "  
		string Code UK "NN"  
		string Description  "NULL"  
		datetime CreatedAt  "NN "  
		datetime UpdatedAt  "NN "  
	}
	Lecture {
		uuid Id PK ""  
		uuid CourseId FK "FK → Course.Id, NN, on delete cascade"  
		int OrderNo  "NN, UK(course_id+order_no)"  
		string Title  "NN"  
		string VideoUrl  "NULL"  
		string Description  "NULL"  
		string Status  "NN, check in ('Draft','Published')"  
		datetime CreatedAt  "NN"  
		datetime UpdatedAt  "NN "  
	}
	Exercise {
		uuid Id PK ""  
		uuid CourseId FK "FK → Course.Id, NN, on delete cascade"  
		int OrderNo "NN; UK(CourseId, OrderNo)" 
		string Title  "NN"  
		string Description  "NULL"  
		datetime CreatedAt  "NN"  
		datetime UpdatedAt  "NN"  
	}
	WorkshopSessions {
		uuid Id PK ""  
		uuid CourseId FK "→ Course.Id, NN, on delete cascade"  
		uuid StudentId FK "→ IDENTITY_USER.Id (Student), NN, on delete restrict"  
		string Description  "NN"  
	}
	MentorWork {
		uuid Id PK ""  
		uuid MentorId FK "→ IDENTITY_USER.Id (Mentor), NN, on delete restrict"  
		string WorkType  "ENUM: Workshop | ExerciseReview"  
		uuid StudentCourseExerciseId FK "NULL when WorkType='Workshop'; ON DELETE RESTRICT"
		uuid WorkshopSessionId  "FK NULL req if Workshop, on delete restrict"  
		datetime StartedAt  ""  
		datetime EndedAt  "NULL"  
		numeric Hours  "auto: (EndedAt-StartedAt)"  
		datetime WorkedAt  ""  
		datetime CreatedAt  ""  
	}
	StudentCourseExercise {
		uuid Id PK "Surrogate key"  
		uuid UserId FK,UK "FK → AspNetUsers.Id (ON DELETE RESTRICT)"  
		uuid CourseId FK,UK "FK → Course.Id (ON DELETE CASCADE)"  
		uuid ExerciseId FK,UK "→ Exercise(CourseId, Id), NN, on delete cascade"  
		string Status  "CHECK IN ('NotDone','Done')"  
		string ChatStorageKind  "CHECK IN ('mongo','s3','fs','url'); default='mongo'"  
	}


	IDENTITY_USER ||--o{ StudentCourseExercise : "UserId"
    IDENTITY_USER ||--o{ MentorWork           : "MentorId"
    IDENTITY_USER ||--o{ StudentTopUp         : "as_student"
    IDENTITY_USER ||--o{ StudentTopUp         : "as_admin"
    IDENTITY_USER ||--o{ WorkshopSessions     : "StudentId"

	Course ||--o{ Lecture            : "contains"
    Course ||--o{ Exercise           : "contains"
    Course ||--o{ WorkshopSessions   : "CourseId"
    Course ||--o{ StudentCourseExercise : "CourseId"

    Exercise ||--o{ StudentCourseExercise : "ExerciseId"
    StudentBalance }|..|| IDENTITY_USER : "aggregates_for"

    StudentCourseExercise ||--o{ MentorWork : "StudentCourseExerciseId"
    WorkshopSessions      ||--o{ MentorWork : "WorkshopSessionId"
```

## Change Log  
- v2 (2025-09-24) — Изменено:
    - UserData заменена IDENTITY_USER (виртуальная таблица —  Identity-managed tables)
    - слит ExerciseChat в StudentExercise
	- слит Journal в StudentExercise
    - добавлена WorkshopSessions — описание воркшопа и связь с конкретным Course
    - описаны основные аттрибутвы полей (ограничения / FK / UK / NN / Null)
- v1 (2025-09-12) —  DRAFT версия.  