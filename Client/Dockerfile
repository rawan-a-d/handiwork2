# Production file
# Make an image that runs the application specifically in production

# 1. Build phase
# install dependencies and build our application (create a compact folder with only what we need)
# will result in a build directory
FROM node:16-alpine as builder

WORKDIR '/app'

COPY ./package.json ./

RUN npm install

COPY . .

# will result in a build folder inside the app directory
# DEVELOPMENT
#RUN npm run build
# PRODUCTION
RUN npm run prod

# 2. Run phase
# https://hub.docker.com/_/nginx
FROM nginx

EXPOSE 80

# Copy nginx ocnfiguration file and replace existing file
COPY ./nginx/default.conf /etc/nginx/conf.d/default.conf
RUN rm -rf /usr/share/nginx/html/*
# Copy build folder from other phase called builder to /user/share/nginx/html
COPY --from=builder /app/dist/HandiWorkClient /usr/share/nginx/html

# we don't need to set up a command, because the default command of the container starts nginx


#FROM nginx:alpine
#COPY /dist/HandiWorkClient /usr/share/nginx/html
