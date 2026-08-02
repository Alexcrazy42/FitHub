# Stage 1: Build
FROM node:20-alpine AS builder
WORKDIR /app

ARG MY_API_URL

ENV VITE_API_URL=$MY_API_URL

# Копируем файлы зависимостей
COPY package*.json ./
COPY yarn.lock* ./
COPY pnpm-lock.yaml* ./

RUN npm ci || npm install
# Или для yarn: RUN yarn install --frozen-lockfile
# Или для pnpm: RUN npm install -g pnpm && pnpm install --frozen-lockfile

# Копируем исходники
COPY . .

# Собираем приложение
RUN npm run build

# Stage 2: Production
FROM nginx:alpine AS production

# Копируем собранные файлы в nginx
COPY --from=builder /app/dist /usr/share/nginx/html

# Копируем кастомный конфиг nginx (опционально)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Добавляем скрипт для запуска с переменными окружения
#COPY docker-entrypoint.sh /docker-entrypoint.sh
#RUN chmod +x /docker-entrypoint.sh

EXPOSE 80

#ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]