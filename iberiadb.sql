-- DROP SCHEMA dbo;

CREATE SCHEMA dbo;
-- iberiadb.dbo.category definition

-- Drop table

-- DROP TABLE iberiadb.dbo.category;

CREATE TABLE iberiadb.dbo.category (
	id int NOT NULL,
	name varchar(255) COLLATE Modern_Spanish_CI_AS NOT NULL,
	CONSTRAINT category_PK PRIMARY KEY (id)
);


-- iberiadb.dbo.[user] definition

-- Drop table

-- DROP TABLE iberiadb.dbo.[user];

CREATE TABLE iberiadb.dbo.[user] (
	id int NOT NULL,
	first_name varchar(255) COLLATE Modern_Spanish_CI_AS NULL,
	last_name varchar(255) COLLATE Modern_Spanish_CI_AS NULL,
	email varchar(255) COLLATE Modern_Spanish_CI_AS NOT NULL,
	password varchar(255) COLLATE Modern_Spanish_CI_AS NOT NULL,
	rol int NOT NULL,
	CONSTRAINT user_PK PRIMARY KEY (id)
);


-- iberiadb.dbo.[order] definition

-- Drop table

-- DROP TABLE iberiadb.dbo.[order];

CREATE TABLE iberiadb.dbo.[order] (
	id int NOT NULL,
	[date] datetime NOT NULL,
	import decimal(38,0) NOT NULL,
	user_id int NOT NULL,
	CONSTRAINT order_PK PRIMARY KEY (id),
	CONSTRAINT order_FK_1 FOREIGN KEY (user_id) REFERENCES iberiadb.dbo.[user](id) ON DELETE CASCADE ON UPDATE CASCADE
);


-- iberiadb.dbo.product definition

-- Drop table

-- DROP TABLE iberiadb.dbo.product;

CREATE TABLE iberiadb.dbo.product (
	id int NOT NULL,
	name varchar(255) COLLATE Modern_Spanish_CI_AS NOT NULL,
	description varchar(255) COLLATE Modern_Spanish_CI_AS NULL,
	category_id int NOT NULL,
	provider_id int NOT NULL,
	stock int NOT NULL,
	price decimal(38,0) NOT NULL,
	iva decimal(38,0) NOT NULL,
	CONSTRAINT product_PK PRIMARY KEY (id),
	CONSTRAINT product_Category_FK FOREIGN KEY (category_id) REFERENCES iberiadb.dbo.category(id) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT product_user_FK FOREIGN KEY (provider_id) REFERENCES iberiadb.dbo.[user](id) ON DELETE CASCADE ON UPDATE CASCADE
);


-- iberiadb.dbo.comments definition

-- Drop table

-- DROP TABLE iberiadb.dbo.comments;

CREATE TABLE iberiadb.dbo.comments (
	id int NOT NULL,
	contens text COLLATE Modern_Spanish_CI_AS NULL,
	user_id int NOT NULL,
	product_id int NOT NULL,
	CONSTRAINT comments_PK PRIMARY KEY (id),
	CONSTRAINT comments_ProductFK FOREIGN KEY (product_id) REFERENCES iberiadb.dbo.product(id) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT comments_User_FK FOREIGN KEY (user_id) REFERENCES iberiadb.dbo.[user](id)
);


-- iberiadb.dbo.[image] definition

-- Drop table

-- DROP TABLE iberiadb.dbo.[image];

CREATE TABLE iberiadb.dbo.[image] (
	id int NOT NULL,
	product_id int NOT NULL,
	[image] image NOT NULL,
	CONSTRAINT image_PK PRIMARY KEY (id),
	CONSTRAINT image_Product_FK FOREIGN KEY (product_id) REFERENCES iberiadb.dbo.product(id) ON DELETE CASCADE ON UPDATE CASCADE
);


-- iberiadb.dbo.ln_order definition

-- Drop table

-- DROP TABLE iberiadb.dbo.ln_order;

CREATE TABLE iberiadb.dbo.ln_order (
	num_order int NOT NULL,
	num_line int NOT NULL,
	quantity int NOT NULL,
	total_import decimal(38,0) NOT NULL,
	ref_product int NOT NULL,
	CONSTRAINT ln_order_PK PRIMARY KEY (num_order,num_line),
	CONSTRAINT ln_order_Order_FK FOREIGN KEY (num_order) REFERENCES iberiadb.dbo.[order](id) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT ln_order_Product_FK FOREIGN KEY (ref_product) REFERENCES iberiadb.dbo.product(id)
);

