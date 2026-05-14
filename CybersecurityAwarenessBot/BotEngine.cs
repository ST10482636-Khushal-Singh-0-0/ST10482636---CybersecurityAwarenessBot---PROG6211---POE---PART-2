using System;
using System.Collections.Generic;

namespace CybersecurityAwarenessBot
{
    // Part 2 Requirement: Delegate for sentiment handling
    public delegate string SentimentModifier(string botResponse);

    public class BotEngine
    {
        public string UserName { get; set; } = "User";

        // Memory features 
        private string _favoriteTopic = string.Empty;
        private string _currentTopic = string.Empty;
        private Random _randomizer;

        // Generic Collections for Topics and Random Responses
        private readonly Dictionary<string, List<string>> _topicResponses;

        public BotEngine()
        {
            _randomizer = new Random();

            // Random Responses & Keyword Recognition using Collections
            _topicResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "password", new List<string> {
                    "Make sure to use strong, unique passwords for each account. Avoid using personal details.",
                    "A good password is at least 12 characters long and includes symbols, numbers, and uppercase letters.",
                    "Consider using a password manager. It remembers your complex passwords so you don't have to!"
                }},
                { "phishing", new List<string> {
                    "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organizations like SARS.",
                    "Never click on unexpected links in SMSes or emails. Always verify by contacting the organization directly.",
                    "Look out for spelling errors and generic greetings in emails—these are common signs of a phishing attempt."
                }},
                { "scam", new List<string> {
                    "If a deal online looks too good to be true, it probably is. Always verify the seller before sending money.",
                    "Beware of people rushing you to make a payment. Scammers use urgency to force you into making mistakes.",
                    "Never share your OTP (One Time Pin) with anyone, even if they claim to be from your bank."
                }},
                { "privacy", new List<string> {
                    "Check your social media settings to ensure your profile is private and only visible to friends.",
                    "Avoid sharing your real-time location or travel plans online to protect your physical and digital privacy.",
                    "Be mindful of the permissions you grant to mobile apps. Not every app needs access to your camera or contacts."
                }}
            };
        }

        public string ProcessInput(string? userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "I didn't quite catch that. Could you try typing something?";
            }

            string normalizedInput = userInput.Trim().ToLower();

            // 1. Memory Setting
            if (normalizedInput.Contains("interested in") || normalizedInput.Contains("favorite"))
            {
                foreach (var topic in _topicResponses.Keys)
                {
                    if (normalizedInput.Contains(topic))
                    {
                        _favoriteTopic = topic;
                        return $"Great! I'll remember that you're interested in {topic}. It's a crucial part of staying safe online.";
                    }
                }
            }

            // 2. Sentiment Detection via Delegate
            SentimentModifier? sentimentModifier = DetectSentiment(normalizedInput);

            // 3. Conversation Flow (Follow-ups)
            if (normalizedInput.Contains("more") || normalizedInput.Contains("another") || normalizedInput.Contains("explain"))
            {
                if (!string.IsNullOrEmpty(_currentTopic))
                {
                    string followUpResponse = GetRandomResponse(_currentTopic);
                    return sentimentModifier != null ? sentimentModifier(followUpResponse) : followUpResponse;
                }
                else
                {
                    return "I'm not sure which topic you'd like more info on. Try asking me about 'scams' or 'passwords' first.";
                }
            }

            // 4. Keyword Recognition & Topic Tracking
            foreach (var topic in _topicResponses.Keys)
            {
                if (normalizedInput.Contains(topic))
                {
                    _currentTopic = topic;
                    string baseResponse = GetRandomResponse(topic);

                    // Memory Recall integration
                    if (topic == _favoriteTopic && _randomizer.Next(0, 2) == 1)
                    {
                        baseResponse = $"As someone interested in {_favoriteTopic}, you should definitely know this: " + baseResponse;
                    }

                    return sentimentModifier != null ? sentimentModifier(baseResponse) : baseResponse;
                }
            }

            // 5. Error Handling / Edge Cases
            return "I'm not sure I understand. Can you try rephrasing? Try asking me about scams, privacy, passwords, or phishing.";
        }

        private string GetRandomResponse(string topic)
        {
            var responses = _topicResponses[topic];
            int index = _randomizer.Next(responses.Count);
            return responses[index];
        }

        private SentimentModifier? DetectSentiment(string input)
        {
            if (input.Contains("worried") || input.Contains("anxious") || input.Contains("scared"))
            {
                return (response) => "It's completely understandable to feel that way. Scammers can be very convincing. Let me share a tip to help you stay safe: \n\n" + response;
            }
            if (input.Contains("frustrated") || input.Contains("confused") || input.Contains("annoyed"))
            {
                return (response) => "Cybersecurity can definitely feel overwhelming, but taking it one step at a time helps. Here is a simple tip for you: \n\n" + response;
            }
            if (input.Contains("curious") || input.Contains("interested"))
            {
                return (response) => "It's great that you want to learn more! Being informed is your best defense. Here is something interesting: \n\n" + response;
            }

            return null;
        }
    }
}