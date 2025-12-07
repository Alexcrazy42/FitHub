delete from gym_trainer where trainers_id = (select id from trainer where user_id = '019ae611-cd54-7105-90db-c199105568a7')

delete from trainer where user_id in (select id from "user" where id = '019ae611-cd54-7105-90db-c199105568a7')

delete from "token" where user_id in (select id from "user" where id = '019ae611-cd54-7105-90db-c199105568a7')

delete from "user" where id = '019ae611-cd54-7105-90db-c199105568a7'