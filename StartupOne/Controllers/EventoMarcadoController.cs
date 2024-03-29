﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StartupOne.Dto.EventoMarcado;
using StartupOne.Models;
using StartupOne.Service;

namespace StartupOne.Controllers
{

    [Route("api/[controller]")]
    public class EventoMarcadoController : Controller
    {
        private EventoMarcadoService _eventosService;

        public EventoMarcadoController(EventoMarcadoService eventosMarcadosService)
        {
            _eventosService = eventosMarcadosService;
        }

        [HttpPost("cadastrar")]
        [Authorize]
        public IActionResult CadastrarEvento([FromBody] EventoMarcadoDto evento)
        {
            try
            {
                return Ok(_eventosService.CadastrarEvento(evento));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("buscar-todos-por-usuario/{idUsuario}")]
        [Authorize]
        public IActionResult ObterTodosEventos([FromRoute] int idUsuario, [FromQuery] string filtro)
        {
            try
            {
                var eventosUsuario = _eventosService.ObterTodosEventos(idUsuario, filtro);
                return Ok(eventosUsuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("buscar-evento/{idUsuario}")]
        [Authorize]
        public IActionResult ObterEvento([FromRoute] int idUsuario)
        {
            try
            {
                var eventosUsuario = _eventosService.ObterEvento(idUsuario);
                return Ok(eventosUsuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("buscar-eventos-dia/{idUsuario}")]
        [Authorize]
        public IActionResult ObterEventoDoDia([FromRoute] int idUsuario, [FromQuery] DateTime data)
        {
            try
            {
                var eventosUsuario = _eventosService.ObterEventoDoDia(idUsuario, data);
                return Ok(eventosUsuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("deletar-evento/{idEvento}")]
        [Authorize]
        public IActionResult DeletarEvento([FromRoute] int idEvento)
        {
            {
                try
                {
                    _eventosService.DeletarEvento(idEvento);
                    return Ok("Evento Excluido com sucesso.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPut("atualizar-evento")]
        [Authorize]
        public IActionResult AtualizarEvento([FromBody] EventoMarcadoDto evento)
        {
            {
                try
                {
                    return Ok(_eventosService.AtualizarEvento(evento));
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPatch("atualizar-conclusao-evento/{idEvento}")]
        [Authorize]
        public IActionResult AtualizarConclusaoEvento([FromRoute] int idEvento)
        {
            {
                try
                {
                    _eventosService.AtualizarConclusaoEvento(idEvento);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}