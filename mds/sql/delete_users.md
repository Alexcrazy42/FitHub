delete from trainer where user_id in (select id from "user" where is_active = false)

delete from "token" where user_id in (select id from "user" where is_active = false)

delete from visitor where user_id in (select id from "user" where is_active = false)

delete from "user" where is_active = false