
# Gym Management System

Gym Management System is a WPF (Windows Presentation Foundation) application designed to manage gym memberships, members, chips, and more. The application provides user-friendly interfaces for managing various aspects of gym operations, including adding, editing, and deleting users and memberships.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Screenshots](#screenshots)
- [Architecture](#architecture)
- [Installation](#installation)
- [Usage](#usage)
- [Technologies Used](#technologies-used)

## Overview

The Gym Management System provides an efficient and organized way to manage gym members, their memberships, and access via chips. It ensures that gym administrators can keep track of all relevant data in one place with ease.

## Features

- **User Management**: Add, edit, and delete gym members with basic information like name, phone number, and email.
- **Membership Management**: Manage memberships, including setting start and end dates.
- **Chip Management**: Assign, update, or delete access chips for members.
- **User Information**: View details of specific users, including the number of memberships and assigned chips.
- **Responsive UI**: Modern, user-friendly design with intuitive controls.

## Screenshots

### Main View
![Main View](https://github.com/xbognar/GymManagementSystemFrontend/blob/master/GymWPF/Resources/Images/MainView.png)

## Add Views

<div align="center">
    <img src="https://github.com/xbognar/GymManagementSystemFrontend/blob/master/GymWPF/Resources/Images/AddMemberView.png" alt="Add Member View" width="400" style="margin: 5px;"/>
    <img src="https://github.com/xbognar/GymManagementSystemFrontend/blob/master/GymWPF/Resources/Images/AddMembershipView.png" alt="Add Membership View" width="400" style="margin: 5px;"/>
</div>

<div align="center">
    <img src="https://github.com/xbognar/GymManagementSystemFrontend/blob/master/GymWPF/Resources/Images/AddChipView.png" alt="Add Chip View" width="400" style="margin: 5px;"/>
    <img src="https://github.com/xbognar/GymManagementSystemFrontend/blob/master/GymWPF/Resources/Images/ChangeChipView.png" alt="Change Chip View" width="400" style="margin: 5px;"/>
</div>

### User Information View
![User Info View](https://github.com/xbognar/GymManagementSystemFrontend/blob/master/GymWPF/Resources/Images/UserInfoView.png)

## Architecture

The Gym Management System follows the MVVM (Model-View-ViewModel) architecture pattern, which enhances maintainability and separation of concerns:

- **Model**: Represents the business logic and data, including the `Member`, `Membership`, and `Chip` classes.
- **View**: The XAML files that define the UI for each component of the system, such as `MainView`, `AddMemberView`, `AddMembershipView`, etc.
- **ViewModel**: Handles the logic and interaction between the Model and View. Examples include `MainViewModel`, `UserInfoViewModel`, `AddMemberViewModel`.

## Installation

To set up the project locally, follow these steps:

1. **Clone the repository**:
   ```bash
   git clone https://github.com/yourusername/GymManagementSystem.git
   ```

2. **Navigate to the project directory**:
   ```bash
   cd GymManagementSystem
   ```

3. **Install dependencies**:
   Make sure that all necessary NuGet packages are restored automatically when opening the solution in Visual Studio.

4. **Set up the environment variables**:
   Create a `.env` file in the project root:
   ```plaintext
   USERNAME=your_username
   PASSWORD=your_password
   ```

5. **Run the application**:
   Open the solution in Visual Studio and start the project.

## Usage

1. **Manage Members**: Add, edit, and delete members using the Add Member view.
2. **Manage Memberships**: Track gym memberships and assign membership types to members.
3. **Manage Chips**: Handle chip assignment for access control using the Chip view.
4. **View User Info**: Use the User Info view to see detailed information about members.

## Technologies Used

- **C# and .NET**: The core programming language and framework.
- **WPF (Windows Presentation Foundation)**: The UI framework for the application.
- **MVVM (Model-View-ViewModel)**: For structured separation of concerns.
- **DotNetEnv**: To load environment variables.
- **Microsoft.Extensions.DependencyInjection**: For dependency injection.
- **CommunityToolkit.Mvvm**: For MVVM support.

