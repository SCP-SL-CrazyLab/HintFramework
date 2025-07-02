
using System;

namespace CrazyHintFramework.API.Models
{
    /// <summary>
    /// Represents single Hint data
    /// </summary>
    public class HintData
    {
    
        /// <summary>
        /// The text to be displayed
        /// </summary>
        public string Text { get; set; }

    
        /// <summary>
        /// The duration of the Hint display in seconds
        /// </summary>
        public float Duration { get; set; }

        
        /// <summary>
        /// Hint priority (higher numbers have greater priority)
        /// </summary>
        public int Priority { get; set; }

     
        /// <summary>
        /// A unique identifier for the Hint
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the person who created this Hint
        /// </summary>
        public string SourcePlugin { get; set; }

        /// <summary>
        /// The time at which the Hint was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The time at which the Hint will expire
        /// </summary>
        public DateTime ExpiresAt { get; set; }

      
        /// <summary>
        /// Is the Hint currently active?
        /// </summary>
        public bool IsActive { get; set; }

     
        /// <summary>
        /// Create a new Hint
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="duration">Duration in seconds</param>
        /// <param name="priority">Priority</param>
        /// <param name="sourcePlugin">Name of the source plugin</param>
        public HintData(string text, float duration, int priority = 0, string sourcePlugin = "Unknown")
        {
            Text = text;
            Duration = duration;
            Priority = priority;
            SourcePlugin = sourcePlugin;
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
            ExpiresAt = CreatedAt.AddSeconds(duration);
            IsActive = true;
        }

    
        /// <summary>
        /// Check if the Hint has expired
        /// </summary>
        /// <returns>true if expired</returns>
        public bool IsExpired()
        {
            return DateTime.Now > ExpiresAt;
        }

    
        /// <summary>
        /// Updates the Hint status based on the current time
        /// </summary>
        public void UpdateStatus()
        {
            if (IsExpired())
            {
                IsActive = false;
            }
        }
    }
}

