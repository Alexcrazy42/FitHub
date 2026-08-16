# Prometheus

Система мониторинга и сбора данных. Следит за работой серверов, программ. Хранит показатели, сохраняет в TimeSeries DB.

Язык запросов - promql

## Типы метрик

counter - счетчик (монотонно растущий)

gauge - значение (меняется во время жизни приложения)

histogram - распределение значений

summary (используется редко)

## Тип данных

### scalar - число

### instant vector

набор временных рядов, каждый с одним значением в один момент времени

Instant Vector: http_requests_duration_seconds{status="200"}

| Labels                      | Value  | Timestamp |
|-----------------------------|--------|-----------|
| {method='GET', status='200'}| 125.3  | t=123     |
| {method='POST',status='200'}| 121.2  | t=123     |
| {method='PUT', status='200'}| 120.2  | t=123     |

### range vector

набор временных рядом, каждый с множеством значений на промежутке времени

Range Vector: http_requests_duration_seconds{status="200"}[5m]

| Labels                      | Values (массив сэмплов)                  |
|-----------------------------|------------------------------------------|
| {method='GET', status='200'}| [120.1@t=93, 122.4@t=103, 125.3@t=123]  |
| {method='POST',status='200'}| [118.7@t=93, 119.9@t=103, 121.2@t=123]  |
| {method='PUT', status='200'}| [117.5@t=93, 119.0@t=103, 120.2@t=123]  |

## Counter

Монотонно возврастающий счетчик, который можно увеличивать или сбрасывать в 0 при рестарте

```
http_requests_total{method="GET", status="200"} 15234
http_requests_total{method="POST", status="500"} 42
```

Ключевые функции:

```
rate(), increase(), irate()
```

### rate

Скорость роста counter

rate(V_range) -> decimal

rate(V_range) = (V_end - V_start) / (t_end - t_start)

### irate

irate(V_range) -> decimal

irate(V_range) = (V_latest - V_previous) / (t_latest - t_previous)

V_latest - самое свежее значение счетчика

V_previous - предпоследнее значение счетчика

### increase

increase(V_range) = V_end - V_start

## Gauge

Произвольное число, которое может расти и падать

Функции - delta, deriv, predict_linear, sum_over_time, stddev_over_time, quantile_over_time, avg_over_time, min_over_time, max_over_time

1) delta - абсолютное изменение

delta (V_range) = V_last - V_first

2) deriv - скорость изменения

deriv(V_range) = (V_last - V_first) / (t_last - t_first)

3) predict_linear - прогнозирование линейное

predict_linear(V_range, t) = V_last + (V_last - V_first) * t / (t_last - t_first)

t - время прогноза вперед (в сек)

4) avg_over_time - среднее

avg_over_time(V_range) = (V_1 + ... + V_n) / n

n - длина V_range

5) max_over_time - максимальное значение

max_over_time(V_range) = Max(V_1, ..., V_n)

6) min_over_time - минимальное

min_over_time(V_range) = Min(V_1, ..., V_n)

7) sum_over_time - сумма

sum_over_time(V_range) = (V_1 + ... + V_n)

8) stddev_over_time - стандартное отклонение

stddev_over_time(V_range) = sqrt(sum( (V_i - mean)^2 / n ))

9) quantile_over_time - квантиль

quantile_over_time(q, V_range) = значение ниже которого лежат q * len(V_range) точек

# Histogram

Тип метрики, измеряющий распределение значений. Гистограмма автоматически создает несколько временных рядов:

1. счетчики для каждого bucket
2. счетчик общегое количества наблюдений (для каждого bucket)
3. сумма всех значений (для каждого bucket)

Конкретный bucket в моменте времени t - counter (instant vector)

bucket[interval] - range vector

Перцентиль - числовое значение, ниже которого находится заданный процент всех данных из набора

Ф-ии: histogram_quantile, rate, increase, sum, avg

## histogram_quantile(q, b) - значение ниже которого лежит q * 100% наблюдений

q - квантиль (от 0 до 1), b - bucket (instant vector)

## rate

rate(bucket[interval]) = (V_last - V_first) / (t_last - t_first)

на выходе получаем для каждого отдельного эл-та range vector значение v, то бишь instant vector

## increase

increase(bucket[interval]) = V_last - V_first

на выходе instant vector

## агрегации с гистограммой

### sum by

Суммаирование по лейблам

sum by (le) (
    rate(http_request_bucket[5m])
)

на вход функции sum by - Instant vector

на выходе - Instant vector

#### разбор

Вход:

| Labels                      | Value  | Timestamp |
|-----------------------------|--------|-----------|
| {method='GET', status='200'}| 125.3  | t=123     |
| {method='GET', status='404'}| 121.3  | t=123     |
| {method='GET', status='403'}| 122.3  | t=123     |
| {method='POST',status='200'}| 121.2  | t=123     |
| {method='PUT', status='200'}| 120.2  | t=123     |


sum by (method) (V_Instant)

->

| {method='GET'}| 125.3 + 121.3 + 122.3
| {method='POST'}| 121.2
| {method='PUT'}| 120.2

## avg

avg(bucket[5m]) = sum(V_i) / n

на вход - instant vector
на входе - instant vector

### разбор

Вход:

| Labels                      | Value  | Timestamp |
|-----------------------------|--------|-----------|
| {method='GET', status='200'}| 125.3  | t=123     |
| {method='GET', status='404'}| 121.3  | t=123     |
| {method='GET', status='403'}| 122.3  | t=123     |
| {method='POST',status='200'}| 121.2  | t=123     |
| {method='PUT', status='200'}| 120.2  | t=123     |

avg (V_Instant) -> (125.3 + 121.3 + 122.3 + 121.2 + 120.2) / 5

## sum

sum(bucket[5m]) = sum(V_i)

на вход - instant vector
на входе - instant vector

### разбор

Вход:

| Labels                      | Value  | Timestamp |
|-----------------------------|--------|-----------|
| {method='GET', status='200'}| 125.3  | t=123     |
| {method='GET', status='404'}| 121.3  | t=123     |
| {method='GET', status='403'}| 122.3  | t=123     |
| {method='POST',status='200'}| 121.2  | t=123     |
| {method='PUT', status='200'}| 120.2  | t=123     |

sum (V_Instant) -> (125.3 + 121.3 + 122.3 + 121.2 + 120.2)