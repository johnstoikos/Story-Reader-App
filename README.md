# Story Reader – Interactive Fairy Tale Application

## Overview
**Story Reader** is a **Windows Forms application built in C# (.NET)** that allows users to browse, listen to, and manage a collection of fairy tales stored in a local SQLite database.  
It features **text-to-speech playback**, **story management tools**, and **online Wikipedia integration** to fetch additional information about stories.

The project follows a **clean layered architecture**:
- **Model Layer** – Data models representing stories and online information  
- **Data Layer** – SQLite database access and repository pattern  
- **Service Layer** – Business logic and external API integration  
- **UI Layer** – Interactive Windows Forms interface for story browsing and playback  

This structure promotes readability, maintainability, and scalability.

---

## Authors
- **Anargyrou Lamprou Aikaterini** 
- **Stoikos Ioannis Panagiotis** 

---

## Key Features
 Browse fairy tales by category  
 Read and listen to stories using Text-to-Speech (TTS)  
 Manage your story library (Add, Edit, Delete)  
 Fetch online story summaries from **Wikipedia**  
 Adjustable voice, rate, and volume controls  
 Clean and responsive Windows Forms interface  

---

## Architecture Overview

### Model Layer
- **Story.cs** – Represents a story (ID, Title, Category, Content).  
- **StoryInfo.cs** – Represents Wikipedia metadata (Title, Description, Summary, URL).  

Both classes are simple **data transfer objects (DTOs)** with no logic, ensuring separation of concerns.

---

### Data Layer (Repository Pattern)
- **DbFactory.cs**
  - Manages the SQLite database file (`Stories.sqlite`).
  - Ensures the database and schema exist (`EnsureCreated()`).
  - Seeds default stories (Cinderella, Red Riding Hood, etc.).

- **IStoryRepository.cs**
  - Defines the CRUD contract for accessing stories.

- **StoryRepositorySqlite.cs**
  - Implements data access using `System.Data.SQLite`.
  - Handles queries like `GetAll()`, `GetByCategory()`, `Create()`, `Update()`, `Delete()`.
  - Wraps SQL exceptions in descriptive messages for better debugging.

---

### Service Layer
- **IStoryService / StoryService**
  - Provides higher-level operations to the UI.
  - Adds an “All” category option.
  - Handles filtering logic and delegates DB operations to the repository.

- **IStoryInfoService / StoryInfoService**
  - Fetches online data from Wikipedia’s REST API.
  - Uses `HttpClient` and JSON parsing (`Newtonsoft.Json.Linq`).
  - Returns summaries and links to relevant Wikipedia articles.

---

### UI Layer (Windows Forms)
#### **MainForm**
- Central interface for browsing and listening to stories.  
- Features:
  - Category selection (ComboBox)  
  - Story list (ListBox)  
  - Content viewer (TextBox)  
  - Voice control (ComboBox)  
  - TTS controls: Play, Pause, Resume, Stop  
  - Status bar updates (“Ready”, “Speaking…”, etc.)  
  - Menu options:
    - `File → Exit`
    - `Tools → Manage Stories`
    - `Tools → Online Info (Wikipedia)`

- Uses asynchronous methods (`async/await`) to keep UI responsive during TTS and API calls.

#### **StoryManagerForm**
- Story management (CRUD) interface with a DataGridView.  
- Allows adding, editing, and deleting stories via `StoryService`.

#### **OnlineInfoForm**
- Displays retrieved Wikipedia data (title, summary, URL).  
- Includes a “Wikipedia” button that opens the article in the default browser.

---

### Program.cs
Entry point for the application.  
- Ensures database setup (`DbFactory.EnsureCreated()`)  
- Initializes repository and services  
- Launches `MainForm` via dependency injection  

---

## Design Patterns & Best Practices
- **Repository Pattern** → Abstracts data layer logic  
- **Dependency Injection** → Loose coupling between UI, services, and repositories  
- **Single Responsibility Principle** → Each class has one clear purpose  
- **Async/Await** → Keeps UI responsive during web and TTS operations  
- **Error Handling** → Clear exception messages and graceful UI recovery  
- **Layered Separation** → No direct SQL or API logic in the UI  

---

## Future Improvements
-  Multilingual TTS (Greek voice integration via OneCore / .NET 6)  
-  Search function to find stories by title or keyword  
-  Import/Export stories (JSON format)  
-  User customization (save preferences for voice, volume, rate)  
-  Adaptation for mobile (Xamarin / MAUI)  

---

## Technologies Used
| Category | Technology |
|-----------|-------------|
| Language | C# (.NET Framework) |
| Database | SQLite |
| UI | Windows Forms |
| API | Wikipedia REST API |
| Libraries | System.Data.SQLite, Newtonsoft.Json, System.Speech |
| Pattern | Repository & Dependency Injection |

---

## How to Run
1. Clone or download the repository.  
2. Open the solution in **Visual Studio**.  
3. Build the project (`Ctrl + Shift + B`).  
4. Run (`F5`) — the database will be created automatically.  

---

## License
This project is provided for **educational and academic purposes**.

---

## Summary
The **Story Reader** project demonstrates strong software engineering principles applied to a beginner-friendly C# application.  
By dividing responsibilities into **Models, Data, Services, and UI**, the app remains modular, testable, and easy to extend — a solid foundation for any future development.
