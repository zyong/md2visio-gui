classDiagram
    direction LR

    %% ======================
    %% 核心领域模型
    %% ======================

    class User {
        +Long id
        +String username
        +String email
        -String passwordHash
        +UserStatus status
        +login()
        +logout()
    }

    class AdminUser {
        +approveOrder(orderId)
        +banUser(userId)
    }

    class CustomerUser {
        +placeOrder()
        +addToCart(productId, qty)
    }

    User <|-- AdminUser
    User <|-- CustomerUser

    class UserStatus {
        <<enumeration>>
        ACTIVE
        DISABLED
        LOCKED
    }

    User --> UserStatus

    %% ======================
    %% 订单与支付
    %% ======================

    class Order {
        +Long id
        +String orderNo
        +OrderStatus status
        +Date createdAt
        +calculateTotal()
        +pay()
        +cancel()
    }

    class OrderItem {
        +Long productId
        +String productName
        +BigDecimal price
        +int quantity
        +subtotal()
    }

    class OrderStatus {
        <<enumeration>>
        CREATED
        PAID
        SHIPPED
        COMPLETED
        CANCELED
    }

    Order "1" *-- "1..*" OrderItem : 包含
    Order --> OrderStatus

    CustomerUser "1" o-- "0..*" Order : 下单

    %% ======================
    %% 支付系统（接口 + 实现）
    %% ======================

    class Payment {
        <<interface>>
        +pay(amount)
        +refund(amount)
    }

    class AlipayPayment {
        +pay(amount)
        +refund(amount)
    }

    class WechatPayment {
        +pay(amount)
        +refund(amount)
    }

    Payment <|.. AlipayPayment
    Payment <|.. WechatPayment

    Order --> Payment : 使用

    %% ======================
    %% 商品与库存
    %% ======================

    class Product {
        +Long id
        +String name
        +BigDecimal price
        +int stock
        +decreaseStock(qty)
    }

    class InventoryService {
        +lockStock(productId, qty)
        +releaseStock(productId, qty)
    }

    OrderItem --> Product
    Order --> InventoryService : 库存校验

    %% ======================
    %% 仓储层（Repository 模式）
    %% ======================

    class Repository~T~ {
        <<interface>>
        +save(T entity)
        +findById(id)
        +delete(id)
    }

    class UserRepository {
        +findByUsername(username)
    }

    class OrderRepository {
        +findByOrderNo(orderNo)
    }

    Repository~User~ <|.. UserRepository
    Repository~Order~ <|.. OrderRepository

    %% ======================
    %% 应用服务层
    %% ======================

    class OrderService {
        -OrderRepository orderRepository
        -InventoryService inventoryService
        -Payment payment
        +createOrder(userId)
        +payOrder(orderId)
    }

    OrderService --> OrderRepository
    OrderService --> InventoryService
    OrderService --> Payment



