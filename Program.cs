using AutoMapper;
using CommandZone.Data;
using CommandZone.Dtos;
using CommandZone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// setting up connection string sql connection string builder class from sqlclient
var sqlConnectionBuilder = new SqlConnectionStringBuilder();
sqlConnectionBuilder.ConnectionString = builder.Configuration.GetConnectionString("DbConnectionString");
sqlConnectionBuilder.UserID = builder.Configuration["UserId"];
sqlConnectionBuilder.Password = builder.Configuration["Password"];

// register db context to use the above connection string
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(sqlConnectionBuilder.ConnectionString));
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("api/v1/command", async(ICommandRepo commandRepo, IMapper mapper)=>{
    var command = await commandRepo.GetAllCommands();
    return Results.Ok(mapper.Map<IEnumerable<ReadCommandDto>>(command));
});
app.MapGet("api/v1/command/{id}", async(ICommandRepo commandRepo, IMapper mapper, [FromRoute]int id)=>{
    var command = await commandRepo.GetCommandById(id);
    if(command !=null){
        return Results.Ok(mapper.Map<ReadCommandDto>(command));
    }
    return Results.NotFound("Command not found");
});
// seach command by howTo
app.MapGet("api/v1/command/search/{searchTerm}", async(ICommandRepo commandRepo, IMapper mapper, string searchTerm)=>{
    var command = await commandRepo.GetAllCommands();
    var search = command.Where(cmd => cmd.HowTo!.ToLower().Contains(searchTerm.Trim().ToLower()));
    if(search == null){
        return Results.NotFound("Command not found");
    }
    return Results.Ok(mapper.Map<IEnumerable<ReadCommandDto>>(search));
});
// create new command
app.MapPost("api/v1/command", async(ICommandRepo commandRepo, IMapper mapper, CreateCommandDto cmd)=>{
    var commandModel = mapper.Map<Command>(cmd);
    await commandRepo.CreateCommand(commandModel);
    await commandRepo.SaveChanges();
    var commandRead = mapper.Map<ReadCommandDto>(commandModel);
    return Results.Created($"api/v1/command/{commandRead.Id}", commandRead);
});

// update command
app.MapPut("api/v1/command/{id}", async(ICommandRepo commandRepo, IMapper mapper, int id, UpdateCommandDto cmd)=>{
    // find command
    var command = await commandRepo.GetCommandById(id);
    if(command == null){
        return Results.NotFound("Command not found");
    }
    mapper.Map(cmd,command);
    await commandRepo.SaveChanges();
    return Results.NoContent();
});

// delete command
app.MapDelete("api/v1/command/{id}", async (ICommandRepo commandRepo, int id)=>{
    var command = await commandRepo.GetCommandById(id);
    if(command == null){
        return Results.NotFound("Command not found");
    }
    commandRepo.DeleteCommand(command);
    return Results.NoContent();
});
app.Run();

