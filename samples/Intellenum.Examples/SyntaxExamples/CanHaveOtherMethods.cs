﻿using System;

namespace Intellenum.Examples.SyntaxExamples
{
     [Intellenum<TimeSpan>]
     public partial struct Duration
     {
         private static Validation Validate(TimeSpan timeSpan) =>
             timeSpan >= TimeSpan.Zero ? Validation.Ok : Validation.Invalid("Cannot be negative");
     
         public Duration DecreaseBy(TimeSpan amount) => Duration.From(Value - amount);
     }
}