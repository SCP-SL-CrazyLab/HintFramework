
using System;

namespace CrazyHintFramework.API.Models
{
    /// <summary>
    /// يمثل بيانات Hint واحد
    /// </summary>
    public class HintData
    {
        /// <summary>
        /// النص الذي سيتم عرضه
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// مدة عرض الـ Hint بالثواني
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// أولوية الـ Hint (الأرقام الأعلى لها أولوية أكبر)
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// معرف فريد للـ Hint
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// اسم البلغن الذي أنشأ هذا الـ Hint
        /// </summary>
        public string SourcePlugin { get; set; }

        /// <summary>
        /// الوقت الذي تم إنشاء الـ Hint فيه
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// الوقت الذي سينتهي فيه الـ Hint
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// هل الـ Hint نشط حاليًا؟
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// إنشاء Hint جديد
        /// </summary>
        /// <param name="text">النص</param>
        /// <param name="duration">المدة بالثواني</param>
        /// <param name="priority">الأولوية</param>
        /// <param name="sourcePlugin">اسم البلغن المصدر</param>
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
        /// التحقق مما إذا كان الـ Hint قد انتهت صلاحيته
        /// </summary>
        /// <returns>true إذا انتهت الصلاحية</returns>
        public bool IsExpired()
        {
            return DateTime.Now > ExpiresAt;
        }

        /// <summary>
        /// تحديث حالة الـ Hint بناءً على الوقت الحالي
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

