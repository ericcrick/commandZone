using AutoMapper;
using CommandZone.Dtos;
using CommandZone.Models;

namespace CommandZone.Profiles
{
    public class CommandProfile: Profile
    {
        public CommandProfile()
        {
            // source => target
            CreateMap<CreateCommandDto,Command>();
            CreateMap<UpdateCommandDto,Command>();
            CreateMap<Command,ReadCommandDto>();
        }
    }
}