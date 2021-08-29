using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{

    [Route("v1/users")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get(
            [FromServices] DataContext context
        )
        {
            var users = await context
                .Users
                .AsNoTracking()
                .ToListAsync();
            return users;
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        //[Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Post(
            [FromServices] DataContext context,
            [FromBody] User model)
        {
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                try
                {
                    //Força o usuário a ser sempre "funcionário'
                    model.Role = "employee";

                    context.Users.Add(model);
                    await context.SaveChangesAsync();

                    //Escodendo a senha
                    model.Password ="";

                    return model;


                    
                }
                catch (Exception)
                {
                    return BadRequest(new { message ="Não foi possível criar o usuário"});
                }
        }


        [HttpPost]
        [Route("login")]
        //ActionResult dinamico pq as vezes ele vai retornar um uusário e as um erro
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromServices] DataContext context,
            [FromBody] User model)
        {

            var user = await context.Users
                .AsNoTracking() //Não precisa trackear o usuário para nada 
                .Where(x => x.Username == model.Username && x.Password == model.Password)
                .FirstOrDefaultAsync();

            if( user == null)
                return NotFound(new { message = "Usuário ou senha inválido!"});
            
            var token = TokenService.GenerateToken(user);

            //Escodendo a senha
            model.Password ="";

            return new 
            {
                user = user, 
                token = token
            };       
            
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put (
            [FromServices] DataContext context,
            int id,
            [FromBody] User model)
            {
                // Verifica se os dados são válidos
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                //Verifica se o Id informando é mesmo do modelo
                if( id != model.Id)
                    return NotFound(new {message = "Usuário não encontrado!"});

                try
                {
                    context.Entry<User>(model).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    return Ok(model);
                }

                catch (Exception)
                {
                    return BadRequest(new {message = "Não foi possível atualizar o usuário"});
                }    
                

            }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Delete(
            [FromServices] DataContext context, 
            int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if(user == null)
                return NotFound( new {menssage = "Usuário não encontrado!"});
            
            try
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return Ok(new {message = "Usuário removido com sucesso"});
            }

            catch (Exception)
            {
                return BadRequest(new {message = "Não foi possivél remover o usuário!"});
            }
        }
    }
    
}