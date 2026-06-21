#!/bin/sh
# docker-entrypoint.sh

# Заменяем плейсхолдеры в собранных JS файлах
find /usr/share/nginx/html -type f -name "*.js" -exec sed -i \
  "s|VITE_API_URL|${VITE_API_URL:-http://platform-api}|g" {} \;

exec nginx -g "daemon off;"