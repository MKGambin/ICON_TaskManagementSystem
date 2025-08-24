ICON Task Management System - Trial Project

This project provides task management functionality, enabling users to manage tasks and users. Users can create, view, edit, and delete users and tasks via a React frontend, with a .NET Core backend handling API requests and database integration.

Key Features

User Management

● CRUD operations for users: create, view, update, and delete.

● User-specific task view: Each user's tasks are filtered and managed separately.

Task Management

● CRUD operations for tasks: create, view, update, and delete.

● Task status tracking: Tasks have statuses (Pending, InProgress, Completed, Cancelled).

Database Integration

● Users and task items are stored in a relational database via Entity Framework Core (code-first approach).

Frontend (React)

● Axios used for API communication.

● Dynamic forms for creating and updating users and tasks.

● Responsive tables with inline actions (Edit/Delete) and back button.

● Basic error handling implemented; pages no longer crash on failed requests.

Unit Testing

● Backend helpers and services have NUnit unit tests (UserHelper, TaskItemHelper) covering most of the logic.

Current Status

● All core functionalities including user and task CRUD, frontend forms, and table views are fully implemented.

● Styling is functional but minimal (CSS tables, spacing, and borders applied).

● Error handling implemented for API calls; pages no longer crash on failed requests.

● Not implemented / skipped items:
  - Containerization (Dockerfile + docker-compose)

  - Deployment (Netlify/Vercel for frontend, Azure/Render for backend)

  - Optional frontend enhancements (unit tests, task prioritization, drag-and-drop)

Setup Instructions

1. Backend

   ● Update appsettings.json with the correct database connection.

3. Frontend

   ● Navigate to the frontend project folder (webreactapp)
   
   ● Install dependencies:
   
       npm install
   ● Start the project:
   
       npm run dev
   ● Access the frontend at the assigned port.

For any questions or clarifications, feel free to reach out.
