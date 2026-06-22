using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Speech.Synthesis; // Requires the System.Speech library/NuGet package

namespace CybersecurityAwarenessBot
{

    /// Interaction logic for MainWindow.xaml. Handles all UI events, animations, and speech.

    public partial class MainWindow : Window
    {
        private BotEngine _bot; // The core logic processor
        private bool _isAwaitingName = true; // Tracks if the system is waiting for the user's initial name input
        private SpeechSynthesizer _synth; // The text-to-speech engine

        public MainWindow()
        {
            InitializeComponent();
            _bot = new BotEngine();

            // Initialize and configure the Voice Synthesizer
            _synth = new SpeechSynthesizer();
            _synth.SetOutputToDefaultAudioDevice();

            DisplayHeader();
            PlayVoiceGreeting(); // Plays the initial greeting.wav file

            AppendMessage("System", "Welcome to your personalized Cybersecurity Awareness Bot! Please enter your name:");
        }

    
        /// Converts text to speech. Filters out emojis to prevent the bot from reading out symbol names.
    
        private void SpeakText(string text)
        {
            try
            {
                // Sanitize the text by removing common UI emojis before passing it to the synthesizer
                string cleanText = text.Replace("✅", "").Replace("❌", "").Replace("🎮", "").Replace("📋", "").Replace("📜", "").Replace("🗑️", "").Replace("➕", "").Replace("*", "");

                // Cancel any currently playing speech to prevent overlapping audio if the user types fast
                _synth.SpeakAsyncCancelAll();
                _synth.SpeakAsync(cleanText);
            }
            catch
            {
                // Catch block left intentionally empty. If the PC has no audio device, the app continues silently.
            }
        }

    
        /// Dynamically creates a new message block, formats it based on the sender, and animates it into the chat panel.
    
        private void AppendMessage(string sender, string message)
        {
            // Create a main container for the specific message
            StackPanel msgPanel = new StackPanel { Margin = new Thickness(0, 5, 0, 20) };

            // Determine the styling/color based on who sent the message
            SolidColorBrush nameColor;
            if (sender == "System") nameColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FFFF")); // Cyan
            else if (sender == "Guardian") nameColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#39FF14")); // Green
            else nameColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD700")); // Yellow

            // Build the Name and Timestamp block
            TextBlock senderBlock = new TextBlock
            {
                Text = $"[{sender.ToUpper()}] // {DateTime.Now:HH:mm}",
                Foreground = nameColor,
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 5)
            };

            // Build the actual message content block
            TextBlock messageBlock = new TextBlock
            {
                Text = message,
                Foreground = Brushes.WhiteSmoke,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 15,
                TextWrapping = TextWrapping.Wrap
            };

            msgPanel.Children.Add(senderBlock);
            msgPanel.Children.Add(messageBlock);

            // Setup the fluid slide-in animation properties
            msgPanel.Opacity = 0; // Start completely transparent
            TranslateTransform slideTransform = new TranslateTransform(0, 15); // Start 15 pixels lower than its final position
            msgPanel.RenderTransform = slideTransform;

            // Add the prepared UI element to the visual tree
            ChatPanel.Children.Add(msgPanel);
            ChatScroll.ScrollToEnd(); // Ensure the user always sees the newest message

            // Define the animation behaviors (Fade in and Slide up over 400 milliseconds)
            DoubleAnimation fadeAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400));
            DoubleAnimation slideAnim = new DoubleAnimation(15, 0, TimeSpan.FromMilliseconds(400))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } // Provides a smooth, natural deceleration effect
            };

            // Trigger the animations
            msgPanel.BeginAnimation(UIElement.OpacityProperty, fadeAnim);
            slideTransform.BeginAnimation(TranslateTransform.YProperty, slideAnim);
        }

    
        /// Displays the custom ASCII art header at the start of the application or when the chat is cleared.
    
        private void DisplayHeader()
        {
            string guardianArt = @"
 _______    __   __     _____    _______   ______    _________    _____    ___   __
/ _____ \  | |   | |   / ___ \   | ___  \  | ___ \   |___  __|   / ___ \   |   \ | |
| |        | |   | |  / /   \ \  | |  | |  | |  \ |     | |     / /   \ \  | |\ \| |
| | _____  | |   | |  | |___| |  | |__| |  | |  | |     | |     | |___| |  | | \ \ |
| | |__ |  | |   | |  | _____ |  | __  /   | |  | |     | |     | _____ |  | |  \ \|
| |   | |  | |   | |  | |   | |  | | \ \   | |  | |     | |     | |   | |  | |   | |
| |___| |  | |___| |  | |   | |  | |  | |  | |__/ |   __| |___  | |   | |  | |   | |
\_______/  \_______/  |_|   |_|  |_|  |_|  |_____/   |_______|  |_|   |_|  |_|   |_|
                                                  
 +++ MZANSI'S CYBERSECURITY AWARENESS ASSISTANT +++
";
            TextBlock artBlock = new TextBlock
            {
                Text = guardianArt + "\n===============================================================================",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF")),
                FontFamily = new FontFamily("Consolas"),
                Margin = new Thickness(0, 0, 0, 20)
            };
            ChatPanel.Children.Add(artBlock);
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("greeting.wav");
                player.LoadAsync();
                player.Play();
            }
            catch (Exception) { /* Fails silently if wav is missing from bin folder */ }
        }

        // --- UI EVENT HANDLERS ---
        private void SendButton_Click(object sender, RoutedEventArgs e) => ProcessInput(UserInputBox.Text);

        private void UserInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Allows the user to press the Enter key to send a message
            if (e.Key == Key.Enter) ProcessInput(UserInputBox.Text);
        }

        private void QuickAction_Click(object sender, RoutedEventArgs e)
        {
            // Extracts the text from the clicked quick-action button, trims the emoji, and processes it as if typed
            if (sender is Button clickedButton)
            {
                string rawText = clickedButton.Content.ToString() ?? "";
                ProcessInput(rawText.Substring(4).Trim());
            }
        }

        private void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            // Close the dashboard overlay to reveal the chat interface
            TaskDashboardOverlay.Visibility = Visibility.Collapsed;
            ChatScroll.Visibility = Visibility.Visible;

            // Pre-fill the input box to assist the user
            UserInputBox.Text = "remind me to ";
            UserInputBox.Focus();
            UserInputBox.CaretIndex = UserInputBox.Text.Length; // Places cursor at the end of the text

            string msg = "What would you like me to remind you about?";
            AppendMessage("System", msg);
            SpeakText(msg);
        }

        private void BtnClearChat_Click(object sender, RoutedEventArgs e)
        {
            ChatPanel.Children.Clear();
            DisplayHeader();

            string msg = "Chat history cleared.";
            AppendMessage("System", msg);
            SpeakText(msg);
        }

    
        /// Central input processor. Routes user text to the BotEngine and handles the response logic.
    
        private void ProcessInput(string input)
        {
            // Ignore empty submissions unless the quiz is active (which relies on button clicks)
            if (string.IsNullOrWhiteSpace(input) && !_bot.IsQuizActive) return;

            // Display user message in chat
            if (!_bot.IsQuizActive)
            {
                AppendMessage(_isAwaitingName ? "User" : _bot.UserName, input);
                UserInputBox.Clear();
            }

            // Initialization state: Capture the user's name first
            if (_isAwaitingName)
            {
                _bot.UserName = input.Trim();
                _isAwaitingName = false;

                string greeting = $"Hello, {_bot.UserName}! I am ready to help.";
                AppendMessage("Guardian", greeting);
                SpeakText(greeting);
            }
            else // Standard state: Process commands
            {
                string response = _bot.ProcessInput(input);

                // Intercept the special token to open the GUI dashboard instead of printing text
                if (response == "[DISPLAY_TASKS]")
                {
                    ShowInteractiveTasks();
                    SpeakText("Opening your interactive task manager.");
                }
                else
                {
                    AppendMessage("Guardian", response);
                    SpeakText(response);
                }

                // Check if the UI needs to switch to "Quiz Mode" layout
                UpdateQuizInterface();
            }
        }

    
        /// Queries the database and dynamically generates WPF UI elements to display the tasks.
    
        private void ShowInteractiveTasks()
        {
            // Swap visibility: Hide chat, show dashboard
            ChatScroll.Visibility = Visibility.Collapsed;
            TaskDashboardOverlay.Visibility = Visibility.Visible;
            InteractiveTaskList.Children.Clear(); // Flush old UI elements

            var tasks = _bot.GetUserTasks();

            // Handle empty database case
            if (tasks.Count == 0)
            {
                InteractiveTaskList.Children.Add(new TextBlock { Text = "You have no pending tasks!", Foreground = Brushes.White, FontFamily = new FontFamily("Consolas"), FontSize = 16, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) });
                return;
            }

            // Dynamically construct a visual "Card" for every task pulled from the database
            foreach (var task in tasks)
            {
                Border taskCard = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F1626")),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD700")),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(15),
                    Margin = new Thickness(0, 0, 0, 10)
                };

                Grid taskGrid = new Grid();
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                string statusIcon = task.IsCompleted ? "✅" : "⏳";
                string dateText = task.ReminderDate.HasValue ? $" (Due: {task.ReminderDate.Value.ToShortDateString()})" : "";

                TextBlock txtTitle = new TextBlock
                {
                    Text = $"{statusIcon} {task.Title}{dateText}",
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 16,
                    VerticalAlignment = VerticalAlignment.Center,
                    // Strike through the text if the task is already completed
                    TextDecorations = task.IsCompleted ? TextDecorations.Strikethrough : null
                };
                Grid.SetColumn(txtTitle, 0);
                taskGrid.Children.Add(txtTitle);

                StackPanel btnPanel = new StackPanel { Orientation = Orientation.Horizontal };
                Grid.SetColumn(btnPanel, 1);

                // Only generate a "Complete" button if the task is currently pending
                if (!task.IsCompleted)
                {
                    Button btnComplete = new Button
                    {
                        Content = "COMPLETE",
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#192841")),
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#39FF14")),
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#39FF14")),
                        Margin = new Thickness(0, 0, 10, 0),
                        Style = (Style)FindResource("InteractiveButton")
                    };
                    // Wire up the button to update the DB, then refresh the UI
                    btnComplete.Click += delegate {
                        _bot.CompleteTask(task.TaskId);
                        ShowInteractiveTasks();
                        SpeakText("Task completed.");
                    };
                    btnPanel.Children.Add(btnComplete);
                }

                // Generate a Delete button for all tasks
                Button btnDelete = new Button
                {
                    Content = "DELETE",
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#192841")),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0055")),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0055")),
                    Style = (Style)FindResource("InteractiveButton")
                };
                // Wire up the button to delete from DB, then refresh the UI
                btnDelete.Click += delegate {
                    _bot.DeleteTask(task.TaskId);
                    ShowInteractiveTasks();
                    SpeakText("Task deleted.");
                };
                btnPanel.Children.Add(btnDelete);

                // Assemble the constructed elements
                taskGrid.Children.Add(btnPanel);
                taskCard.Child = taskGrid;
                InteractiveTaskList.Children.Add(taskCard);
            }
        }

        private void CloseTaskDashboard_Click(object sender, RoutedEventArgs e)
        {
            TaskDashboardOverlay.Visibility = Visibility.Collapsed;
            ChatScroll.Visibility = Visibility.Visible;

            string msg = "Task Manager closed.";
            AppendMessage("Guardian", msg);
            SpeakText(msg);
        }

    
        /// Checks the BotEngine state. If a quiz is running, hides the text box and generates clickable option buttons.
    
        private void UpdateQuizInterface()
        {
            QuizAnswersPanel.Children.Clear(); // Flush old quiz options

            if (_bot.IsQuizActive)
            {
                // Lock standard input to force the user to interact with the quiz buttons
                UserInputBox.IsEnabled = false;
                SendButton.IsEnabled = false;
                QuizAnswersPanel.Visibility = Visibility.Visible;

                var currentQuestion = _bot.GetCurrentQuizQuestion();
                if (currentQuestion != null)
                {
                    // Generate a UI button for each possible answer
                    foreach (var option in currentQuestion.Options)
                    {
                        Button optionButton = new Button
                        {
                            Content = option,
                            Height = 40,
                            Margin = new Thickness(5),
                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#192841")),
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FFFF")),
                            BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FFFF")),
                            Style = (Style)FindResource("InteractiveButton")
                        };
                        // When clicked, send the text of the button directly to the input processor
                        optionButton.Click += (s, e) => ProcessInput(option);
                        QuizAnswersPanel.Children.Add(optionButton);
                    }
                }
            }
            else
            {
                // Restore standard chat input controls when the quiz finishes
                UserInputBox.IsEnabled = true;
                SendButton.IsEnabled = true;
                QuizAnswersPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}s