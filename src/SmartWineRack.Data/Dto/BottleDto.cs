// <copyright file="BottleDto.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Data.Dto
{
    /// <summary>
    /// Bottle DTO.
    /// </summary>
    public class BottleDto
    {
        private string? upcCode;

        /// <summary>
        /// Gets or sets the bottle UPC code.
        /// </summary>
        public string UpcCode
        {
            get => this.upcCode ?? throw new ArgumentNullException(nameof(this.UpcCode));

            set => this.upcCode = value ?? throw new ArgumentNullException(nameof(this.UpcCode));
        }

        /// <summary>
        /// Gets or sets the slot the bottle is stored in.
        /// </summary>
        public int Slot { get; set; }
    }
}
