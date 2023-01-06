using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DigitalTwinTests.Models;

namespace DigitalTwinTests
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            this.CreateMap<IDictionary<string, object>, Slot>();
        }
    }
}
