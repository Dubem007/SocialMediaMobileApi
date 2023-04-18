Social Media App

This is a social media app built with .NET 6.0 using SignalR, Hangfire, PostgreSQL, Docker, and Git.

Overview

The app allows users to create accounts and post updates, follow other users, and receive real-time updates on their activity feed. SignalR is used to enable real-time communication between users, while Hangfire is used to schedule background jobs for tasks such as sending emails and updating the user's feed.

The app uses PostgreSQL as its database, with Entity Framework Core as the ORM. Docker is used to containerize the app and its dependencies for easy deployment, and Git is used for version control.

Getting Started

To get started with the app, follow these steps:

Clone the repository to your local machine

Install Docker and Docker Compose if not already installed
Navigate to the project directory in your terminal
Run docker-compose up --build to build and start the app
Navigate to http://localhost:5000 in your web browser to view the app
Technologies Used
This app was built using the following technologies:

- .NET 6.0
- SignalR
- Hangfire
- PostgreSQL
- Entity Framework Core
- Docker
- Git

Folder Structure

The app's folder structure is as follows:

- /app - contains the .NET app code
- /docker - contains the Docker configuration files
- /scripts - contains scripts for managing the app

Contributing

To contribute to the app, follow these steps:

Fork the repository
Create a new branch with your changes
Submit a pull request with your changes
License
This app is licensed under the MIT License. See the LICENSE file for more details.
