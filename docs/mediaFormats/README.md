# Медиаформаты

Справочник по медиаформатам для веб-разработки — внутреннее устройство кодеков, сравнение алгоритмов сжатия, сценарии применения, HTML5-элементы, React-компоненты и обработка в ASP.NET Core.

---

## Содержание

- [Концепции: контейнер vs кодек](#концепции-контейнер-vs-кодек)
- [Фото](#фото)
- [Аудио](#аудио)
- [Видео](#видео)
- [Адаптивный стриминг: HLS и DASH](#адаптивный-стриминг-hls-и-dash)
- [Обработка в ASP.NET Core](#обработка-в-aspnet-core)
- [Таблица совместимости браузеров](#таблица-совместимости-браузеров)

---

## Концепции: контейнер vs кодек

Это разграничение критично для понимания любого медиаформата.

**Контейнер** (формат файла) — обёртка, которая хранит одну или несколько дорожек (видео, аудио, субтитры, метаданные) вместе с информацией о синхронизации и метаданными. Примеры: `.mp4`, `.webm`, `.mkv`, `.ogg`.

**Кодек** (COder/DECoder) — алгоритм сжатия/распаковки медиаданных. Примеры: H.264, VP9, AV1 (видео); AAC, Opus, MP3 (аудио).

Один контейнер может содержать разные кодеки:
```
video.mp4  →  контейнер MP4 (ISOBMFF)
               ├── видеодорожка: H.264 или H.265 или AV1
               └── аудиодорожка: AAC или MP3 или Opus
```

При указании MIME-типа с параметром `codecs` браузер может принять решение о воспроизведении ещё до загрузки файла:
```
video/mp4; codecs="avc1.42E01E, mp4a.40.2"
           │              │           └── AAC-LC
           │              └── H.264 Baseline Profile Level 3.0
           └── контейнер MP4
```

---

## Фото

### Форматы: подробный разбор

#### JPEG (Joint Photographic Experts Group)
- **Алгоритм**: DCT (дискретное косинусное преобразование) → квантование → энтропийное кодирование (Huffman или арифметическое)
- **Цветовые пространства**: YCbCr (по умолчанию), RGB, CMYK
- **Глубина цвета**: 8 бит на канал (24 бит итого); JPEG XL — до 32 бит
- **Прозрачность**: не поддерживается
- **Анимация**: не поддерживается
- **Профили**: Baseline (построчная развёртка), Progressive (постепенное улучшение качества при загрузке)
- **Артефакты**: блочность и звон (ringing) на высоких степенях сжатия — результат квантования DCT-коэффициентов
- **Метаданные**: EXIF (GPS, камера, экспозиция), IPTC, XMP, ICC-профиль
- **Когда использовать**: фотографии, изображения с плавными градиентами, где потеря качества незаметна. Не подходит для скриншотов, текста, логотипов — видны артефакты

#### PNG (Portable Network Graphics)
- **Алгоритм**: фильтрация строк (Sub, Up, Average, Paeth) → DEFLATE (LZ77 + Huffman)
- **Глубина цвета**: 1, 2, 4, 8, 16 бит на канал; поддерживает grayscale, RGB, RGBA
- **Прозрачность**: полный альфа-канал (8 или 16 бит)
- **Анимация**: только через APNG (расширение, поддерживается всеми современными браузерами)
- **Интерлейсинг**: метод Adam7 — 7 проходов по сетке 8×8
- **Когда использовать**: скриншоты, UI-элементы, логотипы, изображения с текстом, любая графика где важна пиксельная точность, изображения с прозрачностью

#### WebP
- **Разработчик**: Google (2010), основан на VP8
- **Алгоритм**: предиктивное кодирование блоков + трансформация VP8/VP8L
  - Lossy: VP8-фреймы с блочным DCT-предсказанием
  - Lossless: LZ77 + Huffman + трансформация цветового пространства
- **Глубина цвета**: 8 бит на канал
- **Прозрачность**: поддерживается (lossy и lossless)
- **Анимация**: поддерживается (ANIM chunks), замена GIF
- **Выигрыш в размере**: ~26% меньше PNG, ~25–34% меньше JPEG при том же качестве
- **Когда использовать**: веб-сайты с 2023+; использовать как основной формат там, где не нужен AVIF; хорошая замена и JPEG, и PNG

#### AVIF (AV1 Image File Format)
- **Основан на**: видеокодеке AV1 (один кадр или анимация)
- **Алгоритм**: intra-prediction, DCT/ADST, loop filtering — те же инструменты что в AV1-видео
- **Глубина цвета**: 8, 10, 12 бит на канал
- **Цветовые пространства**: sRGB, Display P3, Rec.2020, HDR (PQ, HLG)
- **Прозрачность**: полный альфа-канал
- **Выигрыш**: ~50% меньше JPEG, ~20% меньше WebP при сопоставимом качестве
- **Недостаток**: медленное кодирование (libaom); декодирование требует больше ресурсов CPU
- **Когда использовать**: как лучший вариант в `<picture>` с фолбэком на WebP; особенно выгоден для HDR-контента и изображений с 10-битной глубиной

#### GIF (Graphics Interchange Format)
- **Алгоритм**: LZW-сжатие
- **Палитра**: максимум 256 цветов на кадр (8 бит), что даёт артефакты дизеринга
- **Прозрачность**: бинарная (1 бит) — пиксель либо прозрачен, либо нет
- **Анимация**: встроенная поддержка (GIF89a)
- **Когда использовать**: в 2024+ практически никогда; лучше использовать WebP (анимация) или видео MP4/WebM с `autoplay muted loop` — размер в 10–20 раз меньше

#### SVG (Scalable Vector Graphics)
- **Формат**: XML-описание геометрических примитивов и трансформаций
- **Масштабируемость**: бесконечная без потери качества (векторная математика)
- **Интерактивность**: поддерживает CSS, JavaScript, анимации SMIL и CSS
- **Безопасность**: SVG может содержать `<script>` — никогда не рендерить недоверенные SVG напрямую в DOM, использовать `<img>` или CSP
- **Когда использовать**: иконки, логотипы, диаграммы, любая масштабируемая графика. Не подходит для фотографий

#### JPEG XL (.jxl)
- **Статус**: ратифицирован ISO/IEC 18181 (2022), Chrome убрал поддержку в 2023 (!), Firefox в экспериментальном флаге
- **Особенности**: поддерживает lossless JPEG-перекодирование без потерь (транскодинг из JPEG 1:1), HDR, 32 бит/канал
- **Когда использовать**: пока не готов к продакшену из-за неопределённости с браузерной поддержкой

### Сравнение алгоритмов сжатия изображений

| Критерий | JPEG | PNG | WebP | AVIF |
|----------|------|-----|------|------|
| Алгоритм | DCT + Huffman | DEFLATE | VP8/VP8L | AV1 intra |
| Тип сжатия | С потерями | Без потерь | Оба | Оба |
| Прозрачность | Нет | Да (8/16 бит) | Да | Да |
| Анимация | Нет | APNG | Да | Да |
| HDR | Нет | Нет | Нет | Да |
| Размер (фото, Q80) | 100% | 300–500% | 65–75% | 50–60% |
| Скорость декодинга | Быстро | Быстро | Быстро | Медленно |
| Поддержка браузерами | 100% | 100% | 97%+ | 88%+ |

### Magic bytes (определение формата по содержимому)

Никогда не доверяй расширению файла или заголовку `Content-Type` от клиента — всегда проверяй сигнатуру файла:

| Формат | Смещение | Байты (hex) |
|--------|----------|-------------|
| JPEG | 0 | `FF D8 FF` |
| PNG | 0 | `89 50 4E 47 0D 0A 1A 0A` |
| WebP | 0+8 | `52 49 46 46 ?? ?? ?? ?? 57 45 42 50` |
| GIF | 0 | `47 49 46 38 37 61` или `47 49 46 38 39 61` |
| AVIF | 4 | `66 74 79 70 61 76 69 66` (ftyp avif) |
| SVG | — | XML с `<svg` тегом |

### Рекомендуемые настройки сжатия

| Сценарий | Формат | Качество | Примечание |
|----------|--------|----------|------------|
| Аватары пользователей | WebP | 85 | Фолбэк JPEG Q85 |
| Превью видео (thumbnail) | WebP | 80 | Фолбэк JPEG Q75 |
| Иконки, логотипы | SVG / WebP lossless | — | PNG как фолбэк |
| Фото высокого качества | AVIF | 80 | Фолбэк WebP Q85 → JPEG Q85 |
| OG-изображения (og:image) | JPEG | 85 | JPEG лучше для старых краулеров |
| HDR фото | AVIF | 80 | Единственный вариант для HDR в браузере |

### HTML

```html
<!-- Простое изображение -->
<img src="photo.jpg" alt="Описание" width="800" height="600" loading="lazy" />

<!-- Адаптивное изображение с srcset -->
<img
  src="photo.jpg"
  srcset="photo-400.jpg 400w, photo-800.jpg 800w, photo-1200.jpg 1200w"
  sizes="(max-width: 600px) 400px, (max-width: 1200px) 800px, 1200px"
  alt="Описание"
  loading="lazy"
/>

<!-- Современный формат с фолбэком: AVIF → WebP → JPEG -->
<picture>
  <source srcset="photo.avif" type="image/avif" />
  <source srcset="photo.webp" type="image/webp" />
  <img src="photo.jpg" alt="Описание" loading="lazy" width="800" height="600" />
</picture>

<!-- fetchpriority для LCP-изображений (критично для Core Web Vitals) -->
<img src="hero.jpg" alt="Hero" fetchpriority="high" loading="eager" />
```

### React

```tsx
interface ImageProps {
  src: string;
  alt: string;
  width?: number;
  height?: number;
  className?: string;
  priority?: boolean; // для LCP-изображений
}

const Image: React.FC<ImageProps> = ({ src, alt, width, height, className, priority }) => {
  const [error, setError] = React.useState(false);

  if (error) {
    return <div className="image-fallback">Изображение недоступно</div>;
  }

  return (
    <img
      src={src}
      alt={alt}
      width={width}
      height={height}
      className={className}
      loading={priority ? "eager" : "lazy"}
      fetchPriority={priority ? "high" : "auto"}
      onError={() => setError(true)}
    />
  );
};
```

```tsx
// Автоматический <picture> с AVIF/WebP фолбэком
const PictureImage: React.FC<{ baseSrc: string; alt: string; width?: number; height?: number }> = ({
  baseSrc,
  alt,
  width,
  height,
}) => {
  // baseSrc = "/media/photo.jpg" → "/media/photo.avif", "/media/photo.webp"
  const withoutExt = baseSrc.replace(/\.[^.]+$/, "");

  return (
    <picture>
      <source srcSet={`${withoutExt}.avif`} type="image/avif" />
      <source srcSet={`${withoutExt}.webp`} type="image/webp" />
      <img src={baseSrc} alt={alt} width={width} height={height} loading="lazy" />
    </picture>
  );
};
```

---

## Аудио

### Концепция: контейнер и аудиокодек

```
audio.mp4  →  контейнер MP4
               └── аудиодорожка: AAC

audio.ogg  →  контейнер OGG
               └── аудиодорожка: Vorbis или Opus или FLAC

audio.webm →  контейнер WebM (подмножество Matroska)
               └── аудиодорожка: Vorbis или Opus
```

### Форматы: подробный разбор

#### MP3 (MPEG-1 Audio Layer III)
- **Алгоритм**: MDCT (модифицированное DCT) + психоакустическая модель (маскирование частот) + кодирование Huffman
- **Частота дискретизации**: 8–48 кГц
- **Битрейт**: 8–320 кбит/с; рекомендуемый минимум для музыки — 128 кбит/с; для архивов — 192–320 кбит/с
- **CBR vs VBR**: CBR (постоянный битрейт) — предсказуемый размер; VBR (переменный) — лучшее соотношение качества/размера
- **Задержка (latency)**: ~100–500 мс (не подходит для real-time)
- **Когда использовать**: максимальная совместимость, подкасты, музыка для широкой аудитории

#### AAC (Advanced Audio Coding)
- **Профили**:
  - **AAC-LC** (Low Complexity) — основной профиль, лучше MP3 при том же битрейте
  - **HE-AAC v1** — добавляет SBR (Spectral Band Replication): хорошее качество от 48–80 кбит/с
  - **HE-AAC v2** — добавляет PS (Parametric Stereo): стерео от 24–32 кбит/с
  - **AAC-LD / AAC-ELD** — низкая задержка (20–40 мс), для VoIP
- **Контейнеры**: MP4 (`.m4a`, `.aac`), ADTS (`.aac` raw)
- **Частота дискретизации**: до 96 кГц
- **Когда использовать**: основной формат для iOS/macOS, музыкальные сервисы (Apple Music использует AAC 256 кбит/с), видео (стандартная аудиодорожка в MP4)

#### Opus
- **Разработчик**: IETF (2012), открытый стандарт RFC 6716
- **Внутренние кодеки**: SILK (речь, низкие частоты) + CELT (музыка, полный спектр); автоматическое переключение
- **Битрейт**: 6–510 кбит/с; голос — 12–24 кбит/с, музыка — 64–128 кбит/с
- **Задержка**: 2.5–60 мс (настраивается); лучший выбор для real-time
- **Частота дискретизации**: до 48 кГц
- **Когда использовать**: WebRTC, голосовые сообщения, голосовые чаты, видеозвонки. Используется в Discord, WhatsApp, Telegram

#### OGG Vorbis
- **Алгоритм**: MDCT + психоакустика, открытый аналог MP3
- **Битрейт**: VBR по умолчанию; quality 0–10 (~64–500 кбит/с)
- **Ограничение**: Safari не поддерживает нативно (требует полифилл)
- **Когда использовать**: игры (Godot использует Ogg по умолчанию), Linux-ориентированные проекты. В вебе Opus предпочтительнее

#### FLAC (Free Lossless Audio Codec)
- **Алгоритм**: линейное предсказание (LPC) + кодирование Rice/Golomb
- **Сжатие**: 40–60% от WAV при полном восстановлении
- **Глубина**: 4–32 бит; частота до 655 кГц
- **Когда использовать**: архивирование, мастеринг, аудиофильские приложения. Не для стриминга

#### WAV (Waveform Audio)
- **Формат**: PCM без сжатия (или с ADPCM, но редко)
- **Размер**: 44100 Гц × 16 бит × 2 канала = 1411 кбит/с (~10 МБ/мин)
- **Когда использовать**: только как промежуточный формат при записи/редактировании; в вебе избегать

### Сравнение качества аудиокодеков

| Кодек | Тип | Оптимальный битрейт | Задержка | Речь | Музыка |
|-------|-----|---------------------|----------|------|--------|
| MP3 | Lossy | 128–320 кбит/с | ~500 мс | Хорошо | Хорошо |
| AAC-LC | Lossy | 96–256 кбит/с | ~50 мс | Отлично | Отлично |
| HE-AAC | Lossy | 32–80 кбит/с | ~50 мс | Отлично | Хорошо |
| Opus | Lossy | 12–128 кбит/с | 2–60 мс | Превосходно | Отлично |
| Vorbis | Lossy | 64–500 кбит/с | ~100 мс | Хорошо | Хорошо |
| FLAC | Lossless | 700–1500 кбит/с | — | Идеально | Идеально |
| WAV/PCM | Lossless | ~1411 кбит/с | — | Идеально | Идеально |

### HTML

```html
<!-- Базовый аудиоплеер -->
<audio controls>
  <source src="audio.mp3" type="audio/mpeg" />
  <source src="audio.ogg" type="audio/ogg" />
  Ваш браузер не поддерживает элемент audio.
</audio>

<!-- С явным указанием кодека (браузер не скачивает файл для проверки) -->
<audio controls>
  <source src="voice.webm" type='audio/webm; codecs="opus"' />
  <source src="voice.mp4"  type='audio/mp4; codecs="mp4a.40.2"' />
</audio>

<!-- Фоновое аудио (требует взаимодействия пользователя в большинстве браузеров) -->
<audio autoplay muted loop>
  <source src="ambient.mp3" type="audio/mpeg" />
</audio>
```

### React

```tsx
import React, { useRef, useState, useEffect } from "react";

interface AudioPlayerProps {
  src: string;
  mimeType?: string;
}

const AudioPlayer: React.FC<AudioPlayerProps> = ({
  src,
  mimeType = "audio/mpeg",
}) => {
  const audioRef = useRef<HTMLAudioElement>(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);

  const toggle = () => {
    const audio = audioRef.current;
    if (!audio) return;
    if (isPlaying) {
      audio.pause();
    } else {
      audio.play();
    }
    setIsPlaying(!isPlaying);
  };

  useEffect(() => {
    const audio = audioRef.current;
    if (!audio) return;

    const onTimeUpdate = () => setCurrentTime(audio.currentTime);
    const onLoadedMetadata = () => setDuration(audio.duration);
    const onEnded = () => setIsPlaying(false);

    audio.addEventListener("timeupdate", onTimeUpdate);
    audio.addEventListener("loadedmetadata", onLoadedMetadata);
    audio.addEventListener("ended", onEnded);

    return () => {
      audio.removeEventListener("timeupdate", onTimeUpdate);
      audio.removeEventListener("loadedmetadata", onLoadedMetadata);
      audio.removeEventListener("ended", onEnded);
    };
  }, []);

  const formatTime = (seconds: number) => {
    const m = Math.floor(seconds / 60);
    const s = Math.floor(seconds % 60).toString().padStart(2, "0");
    return `${m}:${s}`;
  };

  return (
    <div className="audio-player">
      <audio ref={audioRef} src={src} preload="metadata">
        <source src={src} type={mimeType} />
      </audio>
      <button onClick={toggle}>{isPlaying ? "Пауза" : "Играть"}</button>
      <input
        type="range"
        min={0}
        max={duration}
        value={currentTime}
        onChange={(e) => {
          const t = Number(e.target.value);
          if (audioRef.current) audioRef.current.currentTime = t;
          setCurrentTime(t);
        }}
      />
      <span>
        {formatTime(currentTime)} / {formatTime(duration)}
      </span>
    </div>
  );
};
```

---

## Видео

### Контейнеры и кодеки

#### Контейнеры

| Контейнер | Расширение | Типичные кодеки | Особенности |
|-----------|------------|-----------------|-------------|
| MP4 (ISOBMFF) | `.mp4`, `.m4v` | H.264, H.265, AV1 + AAC, MP3, Opus | Стандарт де-факто; поддерживается везде |
| WebM | `.webm` | VP8, VP9, AV1 + Vorbis, Opus | Открытый; Google; не поддерживается в старом Safari |
| Matroska | `.mkv` | Любые | Гибкий мультидорожечный контейнер; не поддерживается браузерами нативно |
| OGG | `.ogv` | Theora + Vorbis | Устаревший открытый формат; не используется в продакшене |
| QuickTime | `.mov` | H.264, ProRes, HEVC | Apple; поддерживается Safari и macOS |
| AVI | `.avi` | DivX, Xvid, H.264 | Устаревший; Windows; не для веба |

#### Видеокодеки

**H.264 (AVC — Advanced Video Coding)**
- **Стандарт**: ITU-T H.264 / MPEG-4 Part 10 (2003)
- **Алгоритм**: Intra-предсказание (I-кадры), Inter-предсказание (P и B-кадры), DCT 4×4/8×8, энтропийное кодирование CAVLC/CABAC
- **Профили**:
  - Baseline — без B-кадров, CAVLC; для мобильных устройств и видеозвонков
  - Main — B-кадры, CABAC; стандарт для трансляций
  - High — максимальное сжатие; BD, стриминг
- **Аппаратное декодирование**: есть во всех устройствах с 2010 года
- **Лицензирование**: патентный пул MPEG-LA; бесплатно для конечных пользователей, платно для дистрибуции
- **Когда использовать**: основной формат для видео в вебе; максимальная совместимость включая старые Android/iOS

**H.265 (HEVC — High Efficiency Video Coding)**
- **Стандарт**: ITU-T H.265 / MPEG-H Part 2 (2013)
- **Улучшения над H.264**: CTU до 64×64 (vs 16×16 MB), более гибкое разбиение (QT/MT), SAO-фильтрация
- **Сжатие**: ~40–50% лучше H.264 при том же качестве
- **4K/8K**: основной кодек для UHD-контента
- **HDR**: поддержка HDR10, HLG, Dolby Vision
- **Аппаратное декодирование**: все устройства 2015+; в Safari поддерживается нативно
- **Браузерная поддержка**: Safari — да; Chrome/Firefox — нет (патентные споры)
- **Когда использовать**: Apple-экосистема (Safari), нативные приложения; в вебе — только через Safari

**VP9**
- **Разработчик**: Google (2013), открытый и бесплатный
- **Сжатие**: ~40% лучше VP8, сопоставимо с H.265
- **Аппаратное декодирование**: с 2016–2018 года (Chromebooks, некоторые смартфоны)
- **YouTube**: VP9 является основным кодеком на YouTube
- **Браузерная поддержка**: Chrome, Firefox, Edge, Safari 14.1+
- **Когда использовать**: YouTube-style стриминг, когда нужна альтернатива H.264/H.265 без лицензионных отчислений

**AV1**
- **Разработчик**: Alliance for Open Media (2018) — Google, Mozilla, Microsoft, Apple, Netflix, Amazon
- **Алгоритм**: superblock 128×128, CDEF-фильтр, Restoration Filter, Compound Inter Prediction
- **Сжатие**: ~30% лучше VP9 и H.265; ~50% лучше H.264
- **Недостаток**: CPU-intensive кодирование (libaom медленный; SVT-AV1 быстрее в 20–50x)
- **Аппаратное декодирование**: GPU/SoC с 2020–2022 (Apple M3, Snapdragon 8 Gen 2, Intel Arc)
- **Браузерная поддержка**: Chrome 70+, Firefox 67+, Edge 18+, Safari 16.4+
- **Когда использовать**: стриминговые платформы (Netflix, Disney+ используют AV1); там, где размер файла критичен и есть ресурсы для кодирования

**Сравнение эффективности кодеков (условный bitrate при одинаковом VMAF ~95)**

| Кодек | Относительный битрейт | Скорость кодирования | Аппаратный декодинг |
|-------|-----------------------|----------------------|---------------------|
| H.264 (High) | 100% (базис) | Очень быстро | С 2008 |
| H.265 (Main) | ~55–60% | Медленно | С 2015 |
| VP9 | ~55–65% | Медленно (libvpx) | С 2016 |
| AV1 (SVT-AV1) | ~45–55% | Умеренно | С 2021 |

### Разрешения и рекомендуемые битрейты

| Разрешение | FPS | H.264 (Mbps) | H.265/VP9 (Mbps) | AV1 (Mbps) |
|------------|-----|--------------|------------------|------------|
| 480p (854×480) | 30 | 1.0–1.5 | 0.6–0.9 | 0.5–0.7 |
| 720p (1280×720) | 30 | 2.5–4.0 | 1.5–2.5 | 1.2–2.0 |
| 1080p (1920×1080) | 30 | 5.0–8.0 | 3.0–5.0 | 2.5–4.0 |
| 1080p (1920×1080) | 60 | 8.0–12.0 | 4.5–7.0 | 3.5–6.0 |
| 4K (3840×2160) | 30 | 15–25 | 8–15 | 6–12 |
| 4K (3840×2160) | 60 | 25–40 | 15–25 | 12–20 |

### HTML

```html
<!-- Базовый видеоплеер -->
<video controls width="1280" height="720" poster="thumbnail.jpg">
  <!-- Порядок важен: браузер возьмёт первый поддерживаемый -->
  <source src="video.av1.webm" type='video/webm; codecs="av01.0.05M.08"' />
  <source src="video.vp9.webm" type='video/webm; codecs="vp9"' />
  <source src="video.mp4"      type='video/mp4;  codecs="avc1.42E01E, mp4a.40.2"' />
  Ваш браузер не поддерживает элемент video.
</video>

<!-- Фоновое видео вместо анимированного GIF (в 10–20 раз меньше) -->
<video autoplay muted loop playsinline poster="thumbnail.jpg">
  <source src="background.webm" type="video/webm" />
  <source src="background.mp4"  type="video/mp4" />
</video>
```

### React

```tsx
import React, { useRef, useState } from "react";

interface VideoPlayerProps {
  src: string;
  poster?: string;
  autoPlay?: boolean;
  muted?: boolean;
}

const VideoPlayer: React.FC<VideoPlayerProps> = ({
  src,
  poster,
  autoPlay = false,
  muted = false,
}) => {
  const videoRef = useRef<HTMLVideoElement>(null);
  const [isPlaying, setIsPlaying] = useState(autoPlay);
  const [error, setError] = useState<string | null>(null);

  const toggle = () => {
    const video = videoRef.current;
    if (!video) return;
    if (isPlaying) {
      video.pause();
    } else {
      video.play().catch((e) => setError(e.message));
    }
    setIsPlaying(!isPlaying);
  };

  return (
    <div className="video-player">
      {error && <div className="error">{error}</div>}
      <video
        ref={videoRef}
        src={src}
        poster={poster}
        autoPlay={autoPlay}
        muted={muted}
        playsInline
        onPlay={() => setIsPlaying(true)}
        onPause={() => setIsPlaying(false)}
        onError={() => setError("Не удалось загрузить видео")}
        style={{ width: "100%" }}
      />
      <button onClick={toggle}>{isPlaying ? "Пауза" : "Играть"}</button>
    </div>
  );
};
```

```tsx
// HLS-стриминг через hls.js (обязателен для не-Safari браузеров)
// npm install hls.js
import Hls from "hls.js";
import { useEffect, useRef } from "react";

const HlsPlayer: React.FC<{ src: string; poster?: string }> = ({ src, poster }) => {
  const videoRef = useRef<HTMLVideoElement>(null);

  useEffect(() => {
    const video = videoRef.current;
    if (!video) return;

    if (Hls.isSupported()) {
      const hls = new Hls({
        // Буферизация: 30 сек вперёд, 10 сек назад
        maxBufferLength: 30,
        backBufferLength: 10,
        // Начинать с самого низкого качества для быстрого старта
        startLevel: -1,
      });
      hls.loadSource(src);
      hls.attachMedia(video);
      hls.on(Hls.Events.ERROR, (_, data) => {
        if (data.fatal) hls.destroy();
      });
      return () => hls.destroy();
    } else if (video.canPlayType("application/vnd.apple.mpegurl")) {
      // Нативный HLS (Safari)
      video.src = src;
    }
  }, [src]);

  return (
    <video ref={videoRef} controls poster={poster} style={{ width: "100%" }} />
  );
};
```

---

## Адаптивный стриминг: HLS и DASH

### Принцип работы

Адаптивный битрейт (ABR) — разбивка видео на короткие сегменты (обычно 2–10 сек) нескольких качеств. Плеер динамически переключается между качествами в зависимости от скорости сети.

```
master.m3u8 (манифест)
├── playlist_1080p.m3u8  →  segment_000.ts, segment_001.ts ...
├── playlist_720p.m3u8   →  segment_000.ts, segment_001.ts ...
├── playlist_480p.m3u8   →  segment_000.ts, segment_001.ts ...
└── playlist_audio.m3u8  →  audio_000.aac, audio_001.aac ...
```

### HLS (HTTP Live Streaming)

- **Разработчик**: Apple (2009), стандарт RFC 8216
- **Контейнер сегментов**: MPEG-TS (`.ts`) или fMP4 (`.mp4`, HLS v7+)
- **Кодек**: H.264 (обязательно), H.265 и AAC
- **Длина сегмента**: обычно 2–6 сек; короче = меньше задержка, больше overhead
- **Нативная поддержка**: только Safari (macOS/iOS); остальные браузеры требуют `hls.js`
- **Low-Latency HLS (LL-HLS)**: Apple 2019; задержка до 1–2 сек через частичные сегменты и Push; поддерживается hls.js

**Структура манифеста HLS:**
```m3u8
#EXTM3U
#EXT-X-VERSION:7

#EXT-X-STREAM-INF:BANDWIDTH=5000000,RESOLUTION=1920x1080,CODECS="avc1.640028,mp4a.40.2"
1080p/playlist.m3u8

#EXT-X-STREAM-INF:BANDWIDTH=2500000,RESOLUTION=1280x720,CODECS="avc1.4d401f,mp4a.40.2"
720p/playlist.m3u8

#EXT-X-STREAM-INF:BANDWIDTH=800000,RESOLUTION=854x480,CODECS="avc1.42001f,mp4a.40.2"
480p/playlist.m3u8

#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID="audio",URI="audio/playlist.m3u8"
```

### DASH (Dynamic Adaptive Streaming over HTTP)

- **Стандарт**: ISO/IEC 23009-1
- **Манифест**: MPD (Media Presentation Description) — XML-файл
- **Контейнер сегментов**: fMP4 или WebM
- **Кодеки**: любые (H.264, H.265, VP9, AV1, Opus, AAC)
- **Поддержка браузерами**: через `dash.js` или `Shaka Player`; нет нативной поддержки ни в одном браузере
- **Преимущество перед HLS**: открытый стандарт, поддержка DRM (Widevine + PlayReady + FairPlay через EME), нет привязки к Apple

### HLS vs DASH: когда что использовать

| Критерий | HLS | DASH |
|----------|-----|------|
| Экосистема | Apple / CDN | Открытый стандарт |
| Нативная поддержка | Safari | Нет |
| DRM | FairPlay | Widevine, PlayReady |
| Live-стриминг | Да | Да |
| Low Latency | LL-HLS (~1–2 сек) | DASH-LL (~1–2 сек) |
| Кодеки | H.264, H.265, AAC | Любые |
| Где применяют | Twitch, Apple TV+, большинство CDN | YouTube, Netflix, Disney+ |

---

## Обработка в ASP.NET Core

### Раздача статических медиафайлов

```csharp
// Program.cs — раздача файлов из wwwroot
app.UseStaticFiles();

// Из произвольной директории с кэшированием
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "media")),
    RequestPath = "/media",
    OnPrepareResponse = ctx =>
    {
        // Иммутабельные ресурсы (хэш в имени) — кэш навсегда
        if (ctx.File.Name.Contains(".v") || ctx.Context.Request.Query.ContainsKey("v"))
        {
            ctx.Context.Response.Headers.CacheControl = "public,max-age=31536000,immutable";
        }
        else
        {
            // Медиафайлы без версии — 30 дней с возможностью ревалидации
            ctx.Context.Response.Headers.CacheControl = "public,max-age=2592000";
        }
    }
});
```

### Определение формата по magic bytes (не доверяй Content-Type от клиента)

```csharp
public static class MediaSignatureValidator
{
    // Сигнатуры файлов (magic bytes)
    private static readonly (byte[] Signature, int Offset, string MimeType)[] Signatures =
    [
        ([0xFF, 0xD8, 0xFF], 0, "image/jpeg"),
        ([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A], 0, "image/png"),
        ([0x47, 0x49, 0x46, 0x38], 0, "image/gif"),
        ([0x52, 0x49, 0x46, 0x46], 0, "image/webp"), // + проверка WEBP на смещении 8
        // MP4: 'ftyp' на смещении 4
        ([0x66, 0x74, 0x79, 0x70], 4, "video/mp4"),
        // WebM: EBML header
        ([0x1A, 0x45, 0xDF, 0xA3], 0, "video/webm"),
        // MP3: ID3 или MPEG frame sync
        ([0x49, 0x44, 0x33], 0, "audio/mpeg"),
        ([0xFF, 0xFB], 0, "audio/mpeg"),
        // OGG
        ([0x4F, 0x67, 0x67, 0x53], 0, "audio/ogg"),
        // FLAC
        ([0x66, 0x4C, 0x61, 0x43], 0, "audio/flac"),
        // RIFF/WAV
        ([0x52, 0x49, 0x46, 0x46], 0, "audio/wav"),
    ];

    public static async Task<string?> DetectMimeTypeAsync(Stream stream)
    {
        var header = new byte[16];
        var read = await stream.ReadAsync(header);
        stream.Position = 0;

        if (read < 4) return null;

        // Особая проверка WebP: RIFF....WEBP
        if (header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46
            && read >= 12
            && header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50)
        {
            return "image/webp";
        }

        foreach (var (signature, offset, mimeType) in Signatures)
        {
            if (read < offset + signature.Length) continue;
            if (header.Skip(offset).Take(signature.Length).SequenceEqual(signature))
                return mimeType;
        }

        return null;
    }
}
```

### Эндпоинт загрузки с валидацией

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class MediaController : ControllerBase
{
    private static readonly Dictionary<string, string[]> AllowedTypes = new()
    {
        ["image"] = ["image/jpeg", "image/png", "image/webp", "image/gif", "image/avif"],
        ["audio"] = ["audio/mpeg", "audio/ogg", "audio/webm", "audio/aac", "audio/flac"],
        ["video"] = ["video/mp4", "video/webm"],
    };

    private static readonly Dictionary<string, long> MaxSizes = new()
    {
        ["image"] = 20 * 1024 * 1024,   // 20 МБ
        ["audio"] = 100 * 1024 * 1024,  // 100 МБ
        ["video"] = 500 * 1024 * 1024,  // 500 МБ
    };

    [HttpPost("upload")]
    [RequestSizeLimit(500 * 1024 * 1024)]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string mediaType)
    {
        if (!AllowedTypes.TryGetValue(mediaType, out var allowed))
            return BadRequest("Неизвестный тип медиа");

        if (file.Length > MaxSizes[mediaType])
            return BadRequest($"Файл превышает допустимый размер");

        // Проверяем реальный формат по содержимому файла
        var detectedMime = await MediaSignatureValidator.DetectMimeTypeAsync(file.OpenReadStream());
        if (detectedMime == null || !allowed.Contains(detectedMime))
            return BadRequest($"Недопустимый формат файла");

        // ... сохранение в S3 или на диск
        return Ok(new { url = "..." });
    }
}
```

### Определение MIME-типа по расширению

```csharp
using Microsoft.AspNetCore.StaticFiles;

public static class MediaMime
{
    private static readonly FileExtensionContentTypeProvider _provider = BuildProvider();

    private static FileExtensionContentTypeProvider BuildProvider()
    {
        var p = new FileExtensionContentTypeProvider();
        // Добавляем форматы, которых нет в стандартной карте
        p.Mappings[".opus"] = "audio/ogg; codecs=opus";
        p.Mappings[".avif"] = "image/avif";
        p.Mappings[".m3u8"] = "application/vnd.apple.mpegurl";
        p.Mappings[".mpd"]  = "application/dash+xml";
        return p;
    }

    public static string GetMimeType(string fileName)
    {
        if (!_provider.TryGetContentType(fileName, out var contentType))
            contentType = "application/octet-stream";
        return contentType;
    }
}
```

### Стриминг с поддержкой Range-запросов

Браузер отправляет `Range: bytes=0-` при воспроизведении видео/аудио, чтобы перематывать без полной загрузки. Это обязательно для нормальной работы `<video>` и `<audio>`.

```csharp
[HttpGet("{id}")]
public IActionResult Stream(string id)
{
    var path = GetFilePath(id);
    if (!System.IO.File.Exists(path))
        return NotFound();

    // PhysicalFile с enableRangeProcessing автоматически:
    // 1. Отвечает 206 Partial Content на Range-запросы
    // 2. Добавляет заголовок Accept-Ranges: bytes
    // 3. Устанавливает Content-Range в ответе
    return PhysicalFile(path, MediaMime.GetMimeType(path), enableRangeProcessing: true);
}
```

Пример обмена заголовками при seek:
```
→ GET /api/v1/media/video.mp4
  Range: bytes=10485760-

← HTTP/1.1 206 Partial Content
  Content-Range: bytes 10485760-52428799/52428800
  Accept-Ranges: bytes
  Content-Length: 41943040
```

### Стриминг из S3/Minio через presigned URL

```csharp
// Генерация presigned URL — браузер идёт напрямую в S3, минуя сервер
[HttpGet("{id}/url")]
public async Task<IActionResult> GetPresignedUrl(string id, IS3Service s3)
{
    var url = await s3.GetPresignedUrlAsync(id, expiry: TimeSpan.FromHours(1));
    return Ok(new { url });
}

// Прокси-стриминг через сервер (для приватных файлов с авторизацией)
[HttpGet("{id}/stream")]
public async Task<IActionResult> StreamFromS3(string id, IS3Service s3)
{
    var stream = await s3.GetObjectStreamAsync(id);
    var mimeType = await s3.GetContentTypeAsync(id);

    // Для стриминга из S3 Range-запросы надо обрабатывать вручную
    Response.Headers.AcceptRanges = "bytes";
    return File(stream, mimeType, enableRangeProcessing: true);
}
```

### Ограничения размера файлов

```csharp
// Program.cs
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 500 * 1024 * 1024; // 500 МБ
});

// Kestrel — глобальное ограничение на тело запроса
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 500 * 1024 * 1024;
});
```

Переопределение лимита для конкретного эндпоинта:
```csharp
[HttpPost("upload-video")]
[RequestSizeLimit(2L * 1024 * 1024 * 1024)] // 2 ГБ
[RequestFormLimits(MultipartBodyLengthLimit = 2L * 1024 * 1024 * 1024)]
public async Task<IActionResult> UploadLargeVideo(IFormFile file) { ... }
```

### Обработка изображений (SixLabors.ImageSharp)

```csharp
// Изменение размера и конвертация в WebP
// dotnet add package SixLabors.ImageSharp

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

public class ImageProcessingService
{
    public async Task<Stream> ResizeAndConvertToWebPAsync(
        Stream input,
        int maxWidth,
        int maxHeight,
        int quality = 85)
    {
        using var image = await Image.LoadAsync(input);

        // Пропорциональное уменьшение, никогда не увеличивать
        if (image.Width > maxWidth || image.Height > maxHeight)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(maxWidth, maxHeight),
                Mode = ResizeMode.Max,
            }));
        }

        var output = new MemoryStream();
        await image.SaveAsync(output, new WebpEncoder { Quality = quality });
        output.Position = 0;
        return output;
    }
}
```

### Хелпер валидации

```csharp
public static class MediaValidation
{
    public static readonly string[] ImageMimeTypes =
        ["image/jpeg", "image/png", "image/webp", "image/gif", "image/avif"];

    public static readonly string[] AudioMimeTypes =
        ["audio/mpeg", "audio/ogg", "audio/webm", "audio/aac", "audio/flac"];

    public static readonly string[] VideoMimeTypes =
        ["video/mp4", "video/webm"];

    public static bool IsImage(string mimeType) => ImageMimeTypes.Contains(mimeType);
    public static bool IsAudio(string mimeType) => AudioMimeTypes.Contains(mimeType);
    public static bool IsVideo(string mimeType) => VideoMimeTypes.Contains(mimeType);

    public static string? Validate(IFormFile file, string[] allowedTypes, long maxBytes)
    {
        if (!allowedTypes.Contains(file.ContentType))
            return $"Неподдерживаемый тип '{file.ContentType}'";

        if (file.Length > maxBytes)
            return $"Файл превышает лимит {maxBytes / 1024 / 1024} МБ";

        return null;
    }
}
```

---

## Таблица совместимости браузеров

### Изображения

| Формат | Chrome | Firefox | Safari | Edge | Примечание |
|--------|--------|---------|--------|------|------------|
| JPEG | 1+ | 1+ | 1+ | 12+ | Универсально |
| PNG / APNG | 4+ | 3+ | 8+ | 12+ | APNG — Chrome 59+, Safari 8+ |
| WebP (lossy) | 23+ | 65+ | 14+ | 18+ | |
| WebP (lossless) | 23+ | 65+ | 14+ | 18+ | |
| AVIF | 85+ | 93+ | 16.1+ | 121+ | |
| SVG | 4+ | 3+ | 3.1+ | 12+ | |
| JPEG XL | Убрана | Флаг | Нет | Нет | Нестабильная поддержка |

### Аудио

| Кодек | Chrome | Firefox | Safari | Edge |
|-------|--------|---------|--------|------|
| MP3 | 4+ | 22+ | 3.1+ | 12+ |
| AAC | 4+ | 22+ | 3.1+ | 12+ |
| Opus | 25+ | 15+ | 14+ | 14+ |
| OGG Vorbis | 4+ | 3.5+ | Нет | 17+ |
| FLAC | 56+ | 51+ | 11+ | 16+ |
| WAV (PCM) | 4+ | 3.5+ | 3.1+ | 12+ |

### Видео

| Кодек / Контейнер | Chrome | Firefox | Safari | Edge |
|-------------------|--------|---------|--------|------|
| MP4 / H.264 | 4+ | 35+ | 3.1+ | 12+ |
| MP4 / H.265 (HEVC) | Нет | Нет | 11+ | 18+ (Win11) |
| WebM / VP8 | 25+ | 28+ | 14.1+ | 14+ |
| WebM / VP9 | 38+ | 28+ | 14.1+ | 14+ |
| WebM / AV1 | 70+ | 67+ | 16.4+ | 79+ |
| HLS (.m3u8) | hls.js | hls.js | Нативно | hls.js |
| DASH (.mpd) | dash.js | dash.js | dash.js | dash.js |
