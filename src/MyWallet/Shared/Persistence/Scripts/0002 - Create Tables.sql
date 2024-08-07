CREATE TABLE users
(
    id            CHAR(26)                 NOT NULL PRIMARY KEY,
    name          VARCHAR(50)              NOT NULL,
    email         VARCHAR(150)             NOT NULL UNIQUE,
    password_hash TEXT                     NOT NULL,
    created_at    TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at    TIMESTAMP WITH TIME ZONE
);

CREATE TYPE category_type AS ENUM ('income', 'expense');

CREATE TABLE categories
(
    id         CHAR(26)                 NOT NULL PRIMARY KEY,
    user_id    CHAR(26)                 NOT NULL REFERENCES users (id),
    type       category_type            NOT NULL,
    name       VARCHAR(30)              NOT NULL,
    color      VARCHAR(7)               NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE
);

CREATE TABLE user_categories
(
    user_id     CHAR(26) NOT NULL REFERENCES users (id),
    category_id CHAR(26) NOT NULL REFERENCES categories (id) ON DELETE CASCADE,
    PRIMARY KEY (user_id, category_id)
);