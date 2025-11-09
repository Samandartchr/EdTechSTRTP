-- Active: 1760722986707@@127.0.0.1@3306

CREATE TABLE Quizzes 
(
    quiz_id TEXT PRIMARY KEY,
    creator_id TEXT NOT NULL,
    course_id TEXT NOT NULL,
    quiz_title TEXT NOT NULL,
    quiz_description TEXT,
    quiz_content JSON NOT NULL,
);
