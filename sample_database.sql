-- Active: 1769267375767@@localhost@3306@ecommerce_db
-- Create Sample Database with Users, Products, Stock, Orders, and Shipping Tables
-- This script creates a complete e-commerce database schema with relationships

CREATE DATABASE IF NOT EXISTS ecommerce_db;
USE ecommerce_db;

-- Users migrationhistory
CREATE TABLE _migrationhistory (
    id INT PRIMARY KEY AUTO_INCREMENT,
    domain VARCHAR(50) NOT NULL UNIQUE,
    migration int NOT NULL,
    INDEX idx_domain (domain)
);

-- Users Table - Application users/staff
CREATE TABLE users (
    id INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    full_name VARCHAR(100),
    role ENUM('admin', 'staff', 'customer') DEFAULT 'staff',
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_email (email),
    INDEX idx_username (username)
);

-- Customers Table - Customer information
CREATE TABLE customers (
    id INT PRIMARY KEY AUTO_INCREMENT,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    phone VARCHAR(20),
    shipping_address TEXT NOT NULL,
    billing_address TEXT,
    city VARCHAR(50),
    state VARCHAR(50),
    postal_code VARCHAR(20),
    country VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_email (email),
    INDEX idx_phone (phone)
);

-- Products Table
CREATE TABLE products (
    id INT PRIMARY KEY AUTO_INCREMENT,
    product_name VARCHAR(150) NOT NULL,
    description TEXT,
    category VARCHAR(50),
    price DECIMAL(10, 2) NOT NULL,
    cost DECIMAL(10, 2),
    sku VARCHAR(50) NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_sku (sku),
    INDEX idx_category (category)
);

-- Stock Table - Current inventory levels
CREATE TABLE stock (
    id INT PRIMARY KEY AUTO_INCREMENT,
    product_id INT NOT NULL,
    quantity_on_hand INT NOT NULL DEFAULT 0,
    minimum_quantity INT DEFAULT 10,
    maximum_quantity INT DEFAULT 1000,
    warehouse_location VARCHAR(100),
    last_restocked TIMESTAMP,
    last_updated TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE,
    UNIQUE KEY unique_product_stock (product_id),
    INDEX idx_quantity (quantity_on_hand)
);

-- Stock History Table - Track inventory changes
CREATE TABLE stock_history (
    id INT PRIMARY KEY AUTO_INCREMENT,
    stock_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity_before INT NOT NULL,
    quantity_after INT NOT NULL,
    change_reason ENUM('purchase', 'sale', 'adjustment', 'damage', 'return') NOT NULL,
    notes TEXT,
    changed_by INT,
    changed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (stock_id) REFERENCES stock(id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE,
    FOREIGN KEY (changed_by) REFERENCES users(id) ON DELETE SET NULL,
    INDEX idx_changed_at (changed_at),
    INDEX idx_product_id (product_id)
);

-- Orders Table
CREATE TABLE orders (
    id INT PRIMARY KEY AUTO_INCREMENT,
    customer_id INT NOT NULL,
    order_number VARCHAR(50) NOT NULL UNIQUE,
    order_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    total_amount DECIMAL(12, 2) NOT NULL,
    tax_amount DECIMAL(10, 2) DEFAULT 0,
    discount_amount DECIMAL(10, 2) DEFAULT 0,
    status ENUM('pending', 'confirmed', 'processing', 'shipped', 'delivered', 'cancelled', 'returned') DEFAULT 'pending',
    payment_method VARCHAR(50),
    payment_status ENUM('unpaid', 'paid', 'refunded') DEFAULT 'unpaid',
    special_notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (customer_id) REFERENCES customers(id) ON DELETE RESTRICT,
    INDEX idx_customer_id (customer_id),
    INDEX idx_order_date (order_date),
    INDEX idx_status (status),
    INDEX idx_order_number (order_number)
);

-- Order Items Table - Products in each order
CREATE TABLE order_items (
    id INT PRIMARY KEY AUTO_INCREMENT,
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10, 2) NOT NULL,
    subtotal DECIMAL(12, 2) NOT NULL,
    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE RESTRICT,
    INDEX idx_order_id (order_id),
    INDEX idx_product_id (product_id)
);

-- Shipping Table
CREATE TABLE shipping (
    id INT PRIMARY KEY AUTO_INCREMENT,
    order_id INT NOT NULL,
    shipping_address TEXT NOT NULL,
    shipping_method VARCHAR(50) NOT NULL,
    carrier VARCHAR(50),
    tracking_number VARCHAR(100),
    shipping_date TIMESTAMP,
    estimated_delivery_date DATE,
    actual_delivery_date DATE,
    shipping_cost DECIMAL(10, 2),
    status ENUM('pending', 'picked_up', 'in_transit', 'out_for_delivery', 'delivered', 'failed', 'returned') DEFAULT 'pending',
    signature_required BOOLEAN DEFAULT FALSE,
    special_instructions TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE,
    UNIQUE KEY unique_tracking (tracking_number),
    INDEX idx_order_id (order_id),
    INDEX idx_status (status),
    INDEX idx_shipping_date (shipping_date)
);

-- Sample Data Insertion

-- insert Migration History
INSERT INTO _migrationhistory (domain, migration) VALUES
('ecommerce_db', 1);

-- Insert Users
INSERT INTO users (username, email, password, full_name, role) VALUES
('admin_user', 'admin@company.com', 'hashed_password_123', 'Admin User', 'admin'),
('staff_john', 'john.staff@company.com', 'hashed_password_456', 'John Staff', 'staff'),
('staff_sarah', 'sarah.staff@company.com', 'hashed_password_789', 'Sarah Staff', 'staff');

-- Insert Customers
INSERT INTO customers (first_name, last_name, email, phone, shipping_address, city, state, postal_code, country) VALUES
('John', 'Doe', 'john.doe@email.com', '555-0101', '123 Main Street', 'New York', 'NY', '10001', 'USA'),
('Jane', 'Smith', 'jane.smith@email.com', '555-0102', '456 Oak Avenue', 'Los Angeles', 'CA', '90001', 'USA'),
('Michael', 'Johnson', 'michael.j@email.com', '555-0103', '789 Pine Road', 'Chicago', 'IL', '60601', 'USA'),
('Emily', 'Williams', 'emily.w@email.com', '555-0104', '321 Elm Street', 'Houston', 'TX', '77001', 'USA');

-- Insert Products
INSERT INTO products (product_name, description, category, price, cost, sku) VALUES
('Laptop Computer', 'High-performance laptop with 16GB RAM and 512GB SSD', 'Electronics', 1299.99, 900.00, 'LAPTOP-001'),
('Wireless Mouse', 'Ergonomic wireless mouse with 2.4GHz connection', 'Electronics', 29.99, 15.00, 'MOUSE-001'),
('USB-C Cable', 'Premium USB-C charging and data cable - 2 meters', 'Accessories', 19.99, 5.00, 'USB-C-001'),
('Monitor Stand', 'Adjustable monitor stand for 27-32 inch displays', 'Accessories', 89.99, 40.00, 'STAND-001'),
('Keyboard', 'Mechanical keyboard with RGB lighting', 'Electronics', 149.99, 80.00, 'KEYBOARD-001'),
('Monitor', '27-inch 4K monitor with HDR support', 'Electronics', 599.99, 350.00, 'MONITOR-001'),
('Webcam', '1080p HD webcam with auto-focus', 'Electronics', 79.99, 35.00, 'WEBCAM-001');

-- Insert Stock Records
INSERT INTO stock (product_id, quantity_on_hand, minimum_quantity, maximum_quantity, warehouse_location) VALUES
(1, 45, 5, 100, 'Warehouse A - Shelf A1'),
(2, 250, 50, 500, 'Warehouse A - Shelf B2'),
(3, 500, 100, 1000, 'Warehouse B - Shelf C1'),
(4, 120, 20, 250, 'Warehouse A - Shelf D3'),
(5, 80, 10, 200, 'Warehouse B - Shelf E2'),
(6, 30, 5, 100, 'Warehouse A - Shelf F1'),
(7, 150, 30, 400, 'Warehouse B - Shelf G4');

-- Insert Stock History Records
INSERT INTO stock_history (stock_id, product_id, quantity_before, quantity_after, change_reason, notes, changed_by) VALUES
(1, 1, 50, 45, 'sale', 'Sold 5 laptops to customer', 2),
(2, 2, 255, 250, 'sale', 'Sold 5 wireless mice', 2),
(3, 3, 500, 500, 'adjustment', 'Initial stock count verification', 2),
(4, 4, 120, 120, 'purchase', 'Restocked monitor stands', 3),
(5, 5, 90, 80, 'sale', 'Sold 10 keyboards', 2);

-- Insert Orders
INSERT INTO orders (customer_id, order_number, total_amount, tax_amount, discount_amount, status, payment_method, payment_status) VALUES
(1, 'ORD-001-2024', 1499.97, 120.00, 50.00, 'shipped', 'credit_card', 'paid'),
(2, 'ORD-002-2024', 749.97, 60.00, 0.00, 'processing', 'credit_card', 'paid'),
(3, 'ORD-003-2024', 299.97, 24.00, 25.00, 'confirmed', 'paypal', 'paid'),
(4, 'ORD-004-2024', 1879.95, 150.40, 100.00, 'pending', 'credit_card', 'unpaid');

-- Insert Order Items
INSERT INTO order_items (order_id, product_id, quantity, unit_price, subtotal) VALUES
(1, 1, 1, 1299.99, 1299.99),
(1, 2, 1, 29.99, 29.99),
(2, 6, 1, 599.99, 599.99),
(2, 4, 1, 89.99, 89.99),
(3, 2, 5, 29.99, 149.95),
(3, 3, 5, 19.99, 99.99),
(4, 5, 1, 149.99, 149.99),
(4, 6, 1, 599.99, 599.99),
(4, 7, 2, 79.99, 159.98),
(4, 3, 10, 19.99, 199.99);

-- Insert Shipping Records
INSERT INTO shipping (order_id, shipping_address, shipping_method, carrier, tracking_number, shipping_date, estimated_delivery_date, actual_delivery_date, shipping_cost, status, special_instructions) VALUES
(1, '123 Main Street, New York, NY 10001', 'express', 'FedEx', 'FDX123456789', '2026-01-20', '2026-01-22', '2026-01-22', 25.00, 'delivered', 'Leave at front door if not home'),
(2, '456 Oak Avenue, Los Angeles, CA 90001', 'standard', 'UPS', 'UPS987654321', '2026-01-21', '2026-01-25', NULL, 15.00, 'in_transit', 'Fragile - Handle with care'),
(3, '789 Pine Road, Chicago, IL 60601', 'express', 'DHL', 'DHL456789123', '2026-01-22', '2026-01-23', NULL, 30.00, 'picked_up', 'Signature required'),
(4, '321 Elm Street, Houston, TX 77001', 'standard', 'UPS', 'UPS111222333', '2026-01-23', '2026-01-27', NULL, 15.00, 'pending', 'Call before delivery');
