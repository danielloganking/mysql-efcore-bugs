using System;
using System.ComponentModel.DataAnnotations;

namespace App
{
    public class Entity
    {
        [Key]
        public Guid Id { get; set; }
        
        public DateTimeOffset Example { get; private set; } = default(DateTimeOffset); // EF will automatically replace default value with generated DB value
    }
}
