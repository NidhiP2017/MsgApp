using AutoMapper;
using MsgApp.DTO;
using MsgApp.Models;

namespace MsgApp.Automapper
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<Messages, MessagesDTO>();

        }
    }
}
