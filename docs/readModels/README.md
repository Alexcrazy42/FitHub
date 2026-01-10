# ReadModel для чата



Необходимо реализовать боковую панель с чатами

![read-model](../../images/chat-read-model.png)

В обычной реляционной модели это будет тяжелый запрос для бд (тк будет множество join с группировками, order by и тд)

Поэтому внедряем read-model

```sql
CREATE TABLE public.chat_reading_model (
    id uuid NOT NULL,
    chat_id uuid NOT NULL,
    user_id uuid NOT NULL,
    last_message_id uuid NOT NULL,
    last_message_text text NOT NULL,
    last_message_time timestamptz NOT NULL,
    first_message_time timestamptz NOT NULL,
    unread_count int4 NOT NULL,
    created_at timestamptz NOT NULL,
    updated_at timestamptz NOT NULL,
    CONSTRAINT pk_chat_reading_model PRIMARY KEY (id)
);
```

Подход ходовой, часто используется в индустрии. Позволяет совместить мощь реляционной бд (ACID) и денормализации, и получить лучшее из миров sql и nosql