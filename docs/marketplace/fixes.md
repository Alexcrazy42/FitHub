1. [x] outbox не относится конкретно к marketplace, он относится ко всему, его надо вынести куда-то в общее место (в каждом сервисе), + почему там нет exchangename и routingKey, надо их добавить вместо MessageType
2. [x] PlatformServices/Clients не должны в себе содержать клиентов для BankManager. для этого есть проект BankManager/Clients, которые должен дать метод AddBankManagerClients и PlatformService/Application добавит этот метод
3. [x] IStockReservationRepository не должен содержать метода AddOutboxMessageAsync
4. [x] Проект Clients никак не должен ссылаться на проект Application никогда
