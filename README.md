<p align="center">
  <img src="https://raw.githubusercontent.com/DSivtsov/SeaWind/main/assets/ChatGPSeaWindLogo.png" alt="SeaWind Logo" width="200"/>
</p>

<h1 align="center">SeaWind ASP.NET Project</h1>

<p align="center">
  <a href="https://github.com/username/repo/blob/main/LICENSE">
    <img src="https://img.shields.io/github/license/DSivtsov/SeaWind" alt="License"/>
  </a>
  <a href="https://github.com/username/repo/stargazers">
    <img src="https://img.shields.io/github/stars/DSivtsov/SeaWind?style=social" alt="GitHub Stars"/>
  </a>
</p>

---

## 🚀 About

SeaWind is a sample **ASP.NET Core** project.  
It demonstrates best practices for building modern web apps with C#, .NET, and GitHub Actions CI/CD.

---

## 🔧 Features

- ASP.NET Core Web API  
- Entity Framework Core  
- Authentication & Authorization (JWT)  
- Unit & Integration Tests  
- Docker support  

---

## 📦 Getting Started


# Клонирование репозитория
```bash
git clone https://github.com/DSivtsov/SeaWind.git
```

# Задание секретов
Перед запуском проекта произведи локальную настройку секретов для окружения DEV.
Необходимо создать и настроить локальные файлы внутри директории решения `<локальная_копия_репозитория_SeaWind>`.
Настройки в этих файлах должны быть между собой согласованы и не конфликтовать с другими локальными приложениями (порты и т.п.).

```bash
\docker\.env # смотри пример —  .env.example

\src\backend\Api\appsettings.Development.json # смотри пример —  appsettings.Development.json.example
```

# Запуск проекта для окружения DEV
Смотри документ [WorkshopCode_ProjectRun] (https://github.com/DSivtsov/SeaWind/wiki/WorkshopCode_ProjectRun)

---

## ✅ CI/CD Status

🚧 CI/CD workflow will be added soon (GitHub Actions planned).  
<!-- Later replace this line with a badge, e.g.:
[![.NET Build](https://github.com/username/SeaWind/actions/workflows/dotnet.yml/badge.svg)](https://github.com/username/SeaWind/actions/workflows/dotnet.yml)
-->

---

=======
## 👥 Project Team

[![Tatyana Basargina](https://img.shields.io/badge/Tatyana-Basargina-blue)](team/TatyanaBasargina.md)
[![Denis Sivtsov](https://img.shields.io/badge/Denis-Sivtsov-green)](team/DenisSivtsov.md)
[![Magomed Chapaev](https://img.shields.io/badge/Magomed-Chapaev-blue)](team/MagomedChapaev.md)
[![Oleg Kalashnikov](https://img.shields.io/badge/Oleg-Kalashnikov-green)](team/OlegKalashnikov.md)
[![Aleksei Opeikin](https://img.shields.io/badge/Aleksei-Opeikin-blue)](team/AlekseiOpeikin.md)
[![Petr Liapidevskiy](https://img.shields.io/badge/Petr-Liapidevskiy-green)](team/PetrLiapidevskiy.md)
[![OlegFilippskii](https://img.shields.io/badge/Oleg-Filippskii-blue)](team/OlegFilippskii.md)

---

## 📜 License

This project is licensed under the [MIT License](LICENSE).
