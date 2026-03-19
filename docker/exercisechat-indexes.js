db = db.getSiblingDB("exercisechat");

db.messages.createIndex(
  { threadId: 1, seq: 1 },
  { name: "ix_messages_thread_seq" }
);

db.attachments.createIndex(
  { threadId: 1 },
  { name: "ix_attachments_threadId" }
);

/*
db.attachments.createIndex(
  { messageId: 1 },
  { name: "ix_attachments_messageId" }
);
 */
/* db.messages.createIndex(
    { threadId: 1, createdAt: 1 },
    { name: "ix_messages_thread_createdAt" }
); */

/*
db.attachments.createIndex(
  { threadId: 1, createdAt: -1 },
  { name: "ix_attachments_thread_createdAt_desc" }
);

db.attachments.createIndex(
  { orphanExpiresAt: 1 },
  { expireAfterSeconds: 0, name: "ix_attachments_orphan_ttl" }
);
 */
