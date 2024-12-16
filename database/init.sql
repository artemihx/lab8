create table roles
(
    id SERIAL primary key not null,
    name varchar not null
);

create table users
(
    id SERIAL primary key not null,
    login varchar not null,
    password varchar not null,
    fname varchar not null,
    lname varchar not null,
    mname varchar not null,
    photo varchar,
    doc varchar,
    roleId int references roles(id),
    status bool not null default true
);

create table foods
(
    id SERIAL primary key,
    name varchar not null,
    price numeric not null
);

create table orderStatus
(
    id SERIAL primary key,
    name varchar not null
);

create table shift
(
    id SERIAL primary key,
    date timestamp not null,
    status bool not null
);

create table workersOnShift
(
    id SERIAL primary key,
    shiftId int references shift(id),
    workerId int references users(id)
);

create table tables
(
    id SERIAL PRIMARY KEY
);

create table orders
(
    id SERIAL primary key,
    date timestamp not null,
    status int references orderStatus(id),
    tableId int references tables(id),
    shiftId int references shift(id)
);

create table waiterOntables
(
    id SERIAL primary key,
    idWaiter int references users(id),
    idTable int references tables(id)
);

create table foodOnOrders
(
    id SERIAL primary key,
    idFood int references foods(id),
    idOrder int references orders(id)
);


INSERT INTO roles (id,name) VALUES (2,'Официант'), (3,'Повар'), (1,'Админ');

INSERT INTO users (id ,login, password, fname, lname, mname, doc, roleId, status) VALUES
(1,'waiter1', 'waiterpass', 'Иван', 'Иванов', 'Иванович', 'doc1.pdf', 2, TRUE),
(2,'cooker1', 'cookerpass', 'Петр', 'Петров', 'Петрович',  'doc2.pdf', 3, TRUE),
(3,'admin1', 'adminpass', 'Алексей', 'Алексеев', 'Алексеевич', 'doc3.pdf', 1, TRUE);

INSERT INTO foods (name, price) VALUES
 ('Борщ', 100),
 ('Пельмени', 150),
 ('Котлеты', 200),
 ('Салат Оливье', 150),
 ('Плов', 100),
 ('Блины', 50),
 ('Сырники', 50),
 ('Компот', 20),
 ('Чай', 25),
 ('Кофе', 25);

INSERT INTO orderStatus (name) VALUES ('Ожидает'), ('Готовится'), ('Готов'), ('Оплачен'), ('Отменен');

INSERT INTO tables (id) VALUES (1), (2), (3), (4), (5);
