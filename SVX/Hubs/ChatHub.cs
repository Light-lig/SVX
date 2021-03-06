using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SVX.Models;
using System.Web.Mvc;
using SVX.DTO;
using System.Threading.Tasks;

namespace SVX.Hubs
{
    public class ChatHub : Hub
    {
        private ProyectoWeb2021Entities contexto;
        public ChatHub()
        {
            contexto = new ProyectoWeb2021Entities();
        }
        public void readConversations(int id)
        {


            List<Conversacion> lista = new List<Conversacion>();


            var conversaciones = (from c in contexto.Conversacion
                                  join m in contexto.Mensaje on c.idConversacion equals m.idConversacion
                                  join u in contexto.Usuario on m.idFrom equals u.idUsuario
                                  join us in contexto.Usuario on m.idTo equals us.idUsuario
                                  where (u.idUsuario == id || us.idUsuario == id) 
                                  select c
                                  ).ToList() ;

            foreach (var item in conversaciones)
            {
                if (!lista.Contains(item))
                {
                    lista.Add(item);
                }
            }


            Clients.Client(Context.ConnectionId).getConversations(lista);
      
          
        }
        public  void joinRoom(string roomName)
        {
             Groups.Add(Context.ConnectionId, roomName);
        }

        public void readMenesajes(int? idConversacion = null )
        {
            var mensajes = contexto.Mensaje.Where(m => m.idConversacion == idConversacion);
            Clients.Group(idConversacion.ToString()).getMensajes(mensajes);
        }

        public void send(int idTo, int idConversacion, int idFrom,string nombreFrom, string mensaje, string conversacion, string idAnuncio)
        {
            Conversacion con = new Conversacion();
           
            if(idConversacion == 0)
            {
                con.nombre = conversacion;
                contexto.Conversacion.Add(con);
                contexto.SaveChanges();
                con = contexto.Conversacion.ToList().LastOrDefault();
                idConversacion = con.idConversacion;
                joinRoom(con.idConversacion.ToString());
            }
            Mensaje men = new Mensaje();
           
            men.idTo = idTo;
            men.idConversacion = idConversacion;
            men.idFrom = idFrom;
            men.mensaje1 = mensaje;
            men.fecha = System.DateTime.Now;
            men.idAnuncio = idAnuncio;
            contexto.Mensaje.Add(men);
            contexto.SaveChanges();
            Clients.Group(idConversacion.ToString()).addMensajes(idTo, idConversacion, idFrom, nombreFrom, mensaje, System.DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"), idAnuncio);
        }
    }
}