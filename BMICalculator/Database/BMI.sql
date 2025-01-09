create database bmi_calculator;
use bmi_calculator; 
create table users(
id int auto_increment primary key,
username varchar(50) not null unique,
password varchar(100) not null,
age int,
gender varchar(10)
);
create table bmi_history(
id int auto_increment primary key,
user_id int,
height decimal(5,2),
weight decimal(5,2),
bmi decimal(5,2),
date timestamp default current_timestamp,
foreign key(user_id) references users(id)
);

alter table bmi_history
rename column id to S_No;

select * from users;
select * from bmi_history;