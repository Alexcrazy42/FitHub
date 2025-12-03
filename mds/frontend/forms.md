UniversalSelect
    Режимы:
    static — статичные данные с фронта +
    prefetch — единоразовая загрузка с бэкенда +
    infinite — пагинация с виртуальным списком +
    autocomplete — поиск с автокомплитом +
    cascader — каскадный селектор (страна → город → район)
    tree — иерархический выбор (категории с подкатегориями)
    grouped — группировка опций
    tags — мультиселект с созданием новых тегов
    dependent — зависимый от другого селектора
    remoteMultiple — множественный выбор с подгрузкой


UniversalTable
    Режимы:
    static — статичные данные с фронта
    server-side — серверная пагинация, сортировка, фильтрация
    infinite — бесконечный скролл
    virtual — виртуализация для больших списков
    editable — inline редактирование ячеек
    expandable — раскрывающиеся строки
    tree — древовидная структура
    groupable — группировка по колонке


UniversalForm
    Режимы:
    create — создание нового объекта
    edit — редактирование существующего
    view — read-only режим просмотра
    wizard — многошаговая форма
    dynamic — динамические условные поля
    bulk — массовое редактирование
    draft — автосохранение черновика


UniversalTree
    Режимы:
    static — статичное дерево
    lazy — ленивая загрузка веток
    searchable — поиск по узлам с подсветкой
    draggable — перетаскивание узлов
    checkable — выбор множества узлов
    virtual — виртуализация для больших деревьев

UniversalUpload
    Режимы:
    single — один файл
    multiple — несколько файлов
    drag-drop — drag & drop область
    avatar — загрузка аватара с кропом
    chunked — чанковая загрузка больших файлов
    s3-direct — прямая загрузка в S3 (presigned URL)
    image-editor — встроенный редактор изображений

UniversalModal
    Режимы:
    info — информационная модалка
    confirm — подтверждение действия
    form — модалка с формой
    wizard — многошаговый процесс
    fullscreen — полноэкранная модалка
    drawer — боковая панель

UniversalFilter
    Режимы:
    inline — фильтры в строке над таблицей
    drawer — боковая панель с фильтрами
    modal — модалка с расширенными фильтрами
    smart — сохранение в URL query params
    preset — пресеты фильтров

UniversalChart
    Режимы:
    line — линейный график
    bar — столбчатая диаграмма
    pie — круговая диаграмма
    realtime — график в реальном времени (WebSocket)
    heatmap — тепловая карта
    area — график с областями
    scatter — точечная диаграмма

UniversalList
    Режимы:
    simple — простой список
    infinite — infinite scroll
    virtual — виртуализация
    grid — сетка карточек
    masonry — masonry layout (Pinterest)
    kanban — канбан-доска

UniversalDatePicker
    Режимы:
    single — выбор одной даты
    range — диапазон дат
    multiple — несколько дат
    week — выбор недели
    month — выбор месяца
    quarter — выбор квартала
    year — выбор года
    datetime — дата и время
    time — только время

UniversalTransfer
    Режимы:
    simple — простой перенос элементов
    tree — перенос древовидных структур
    search — с поиском в обеих панелях
    pagination — с пагинацией списков
    lazy — ленивая подгрузка