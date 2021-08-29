
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Models;

//Controller tem a função de manipular ou gerenciar as requisições de nossa API

[Route("v1/category")]
public class CategoryController : ControllerBase 
{

    [HttpGet]
    [Route ("")]
    [AllowAnonymous]
    [ResponseCache (VaryByHeader = "User-Agente",
        Location = ResponseCacheLocation.Any,
        Duration = 30)]
    //Para dizer que o método não tem cache caso o cache esteja ativado no startup.cs    
    //[ResponseCache(Duration =0, Location = ResponseCacheLocation.Nome, NoStore = true)]    
    public async Task<ActionResult<List<Category>>> Get( 
        [FromServices] DataContext context
    )
    {
        var categories = await context.Categories.AsNoTracking().ToListAsync();
        return Ok(categories);
    }

    [HttpGet]
    [Route("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Category>> GetById(
        int id,
        [FromServices] DataContext context
    )
    {
    
        var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
       
        return Ok(category); 

    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = "employee")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<List<Category>>> Post(
        [FromBody]Category model,
        [FromServices] DataContext context
        )
    {
        //verifica se o modelo é válido
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Categories.Add(model);
            await context.SaveChangesAsync();

            return Ok(model);
        }
        catch
        {
            
            return BadRequest(new { menssage = "Não foi possivél criar a categoria"});
        }
       
    }
    
    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<Category>>> Put(
        int id, 
        [FromBody] Category model, 
        [FromServices] DataContext context
    )
    {
        //Verifica se o Id informado é o mesmo do modelo
        if (id != model.Id)
            return NotFound( new {message = "Categoria não encontrada, informe um id válido." });

        //verifica se o modelo é válido 
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Entry<Category>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new {menssage ="Este registro já foi atualizado"});
        } 
        catch (Exception)
        {
            return BadRequest(new {menssage ="Não foi possível atualizar a categoria"});
        }    

       
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<List<Category>>> Delete(
        int id,
        [FromServices] DataContext context
    )
    {   var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if(category == null)
                return NotFound(new {menssage = "Categoria não encontra!"});
        try 
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return Ok(new {message = "Categoria removida com sucesso!"}); 
        }
        catch(Exception)
        {
            return BadRequest(new {message = "Não foi possivél remover a categoria!"});
        }

        
    }

}