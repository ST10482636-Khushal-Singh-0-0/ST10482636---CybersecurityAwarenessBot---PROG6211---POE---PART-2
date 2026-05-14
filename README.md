# Mzansi's Cybersecurity Awareness Assistant

A desktop Graphical User Interface (GUI) application built with C# and Windows Presentation Foundation (WPF). This interactive chatbot is designed to educate users about common online threats—such as phishing, scams, and identity theft—with a specific focus on the South African digital landscape.

## Features

* **Interactive GUI:** A clean, responsive chat interface built using WPF and XAML, featuring custom ASCII art branding and auto-scrolling chat history.
* **Dynamic Keyword Recognition:** Identifies user queries related to specific topics (e.g., passwords, privacy, phishing) and serves randomized educational responses using generic C# collections (`Dictionary` and `List`).
* **Sentiment Detection:** Analyzes the user's emotional tone (e.g., worried, frustrated, curious) and utilizes **C# Delegates** to dynamically alter the bot's responses to be more empathetic or encouraging.
* **Conversation Memory:** Remembers the user's name and their favorite cybersecurity topics, recalling this information later in the conversation for a personalized experience.
* **Audio Integration:** Automatically plays a localized voice greeting upon launch using `System.Media.SoundPlayer`.
* **Seamless Flow:** Handles follow-up prompts like "tell me more" or "another tip" without losing track of the current conversation context.

## Technologies Used

* **Language:** C# 10.0+
* **Framework:** .NET 8.0 / 10.0 (Windows)
* **UI Technology:** Windows Presentation Foundation (WPF) / XAML
* **IDE:** Visual Studio 2022

## Project Structure

* `MainWindow.xaml`: Defines the visual layout, chat windows, text boxes, and buttons using XAML.
* `MainWindow.xaml.cs`: The code-behind that links the user interface to the bot's logic and handles UI events (button clicks, keyboard presses, audio playback).
* `BotEngine.cs`: The core logic engine. Handles string manipulation, keyword mapping, state memory, and sentiment analysis.
* `greeting.wav`: The audio file used for the startup sequence.

## How to Run Locally

1. **Clone the Repository:**
   ```bash
   git clone [https://github.com/yourusername/CybersecurityAwarenessBot.git](https://github.com/yourusername/CybersecurityAwarenessBot.git)
