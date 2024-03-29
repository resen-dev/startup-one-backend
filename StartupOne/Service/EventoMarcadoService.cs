﻿using StartupOne.Dto.EventoMarcado;
using StartupOne.Models;
using StartupOne.Repository;
using System.Collections.Generic;

namespace StartupOne.Service
{
    public class EventoMarcadoService
    {
        private readonly EventoMarcadoRepository _eventosRepository;

        private readonly TokenService _tokenService;

        public EventoMarcadoService(EventoMarcadoRepository eventosMarcadosRepository, TokenService tokenService)
        {
            _eventosRepository = eventosMarcadosRepository;
            _tokenService = tokenService;
        }

        public void ValidarEventoMarcado(EventoMarcado eventoMarcado)
        {
            if(eventoMarcado == null)
                throw new Exception("Evento não foi encontrado.");

            if (eventoMarcado.Nome.Length > 30)
                throw new Exception("Nome tem máximo de 30 caracteres");

            if (eventoMarcado.IdUsuario != _tokenService.GetUserIdFromToken())
                throw new Exception("Você não tem permissão para modificar este evento.");

            if (eventoMarcado.Fim.DayOfYear != eventoMarcado.Inicio.DayOfYear)
                throw new Exception("Data inicio e fim devem ser no mesmo dia.");

            if (eventoMarcado.Fim == eventoMarcado.Inicio)
                throw new Exception("Hora fim e início não podem ser iguais.");

            if (eventoMarcado.Fim < eventoMarcado.Inicio)
                throw new Exception("Hora fim não pode ser menor do que a data início.");

            //if(eventoMarcado.Inicio.Minute % 5 != 0 || eventoMarcado.Fim.Minute % 5 != 0)
            //    throw new Exception("Os horários de início e fim devem ser múltiplos de 5.");

            if (_eventosRepository.JaExisteEventoNoPeriodo(eventoMarcado))
                throw new Exception("Já existe evento neste periodo.");

            if(eventoMarcado.Confirmado != true)
                eventoMarcado.Concluido = true;

        }

        public EventoMarcadoDto CadastrarEvento(EventoMarcadoDto eventoDto)
        {

            EventoMarcado evento = new EventoMarcado(
                idEventoMarcado: 0,
                idUsuario: _tokenService.GetUserIdFromToken(),
                nome: eventoDto.Nome,
                inicio: eventoDto.Inicio,
                fim: eventoDto.Fim,
                categoria: eventoDto.Categoria,
                recorrente: null,
                confirmado: eventoDto.Confirmado,
                concluido: eventoDto.Concluido
            );

            ValidarEventoMarcado(evento);
            
            _eventosRepository.Adicionar(evento);

            return new EventoMarcadoDto
            {
                IdEventoMarcado = evento.IdEventoMarcado,
                Inicio = evento.Inicio,
                Fim = evento.Fim,
                Nome = evento.Nome,
                Confirmado = evento.Confirmado,
                Concluido = evento.Concluido,
                Categoria = evento.Categoria
            };
        }

        public EventoMarcadoDto AtualizarEvento(EventoMarcadoDto eventoDto)
        {

            EventoMarcado eventoEditado = _eventosRepository.Obter(eventoDto.IdEventoMarcado);

            eventoEditado.Inicio = eventoDto.Inicio;
            eventoEditado.Fim = eventoDto.Fim;
            eventoEditado.Nome = eventoDto.Nome;

            ValidarEventoMarcado(eventoEditado);

            _eventosRepository.Atualizar(eventoEditado);

            return eventoDto;
        }

        public void DeletarEvento(int idEvento)
        {
            var evento = _eventosRepository.Obter(idEvento);

            if (evento == null) throw new Exception("O evento não foi encontrado");

            if (evento.IdUsuario != _tokenService.GetUserIdFromToken()) throw new UnauthorizedAccessException("Você não tem permissão para excluir este evento.");

            _eventosRepository.Remover(evento);
        }

        public EventoMarcado ObterEvento(int idEvento)
        {
            return _eventosRepository.Obter(idEvento);
        }

        public IEnumerable<EventoMarcadoDto> ObterTodosEventos(int idUsuario, string filtro)
        {
            ICollection<EventoMarcado> eventos;

            if (filtro == "pendentes") eventos = _eventosRepository.ObterEventosPendentesDoUsuario(idUsuario);

            else if(filtro == "atrasados") eventos = _eventosRepository.ObterEventosAtrasadosDoUsuario(idUsuario);

            else if (filtro == "concluidos") eventos = _eventosRepository.ObterEventosConcluidosDoUsuario(idUsuario);

            else eventos = _eventosRepository.ObterTodosEventosDoUsuario(idUsuario);

            IEnumerable<EventoMarcadoDto> eventosDtos = eventos.Select(e => new EventoMarcadoDto
            {
                IdEventoMarcado = e.IdEventoMarcado,
                Inicio = e.Inicio,
                Fim = e.Fim,
                Nome = e.Nome,
                Confirmado = e.Confirmado,
                Concluido = e.Concluido,
                Categoria = e.Categoria
            });

            return eventosDtos;
        }

        public void AtualizarConclusaoEvento(int idEvento)
        {
            EventoMarcado evento = _eventosRepository.Obter(idEvento);

            if (evento == null) throw new Exception("Evento não encontrado");

            if(evento.IdUsuario != _tokenService.GetUserIdFromToken()) throw new UnauthorizedAccessException("Você não tem permissão para alterar este evento.");

            evento.Concluido = !evento.Concluido;

            _eventosRepository.Atualizar(evento);
        }

        public IEnumerable<EventoMarcadoDto> ObterEventoDoDia(int idUsuario, DateTime data)
        {
            ICollection<EventoMarcado> eventos = _eventosRepository.ObterEventoDoDia(idUsuario, data);

            return eventos.Select(e => new EventoMarcadoDto
            {
                IdEventoMarcado = e.IdEventoMarcado,
                Inicio = e.Inicio,
                Fim = e.Fim,
                Nome = e.Nome,
                Confirmado = e.Confirmado,
                Concluido = e.Concluido,
                Categoria = e.Categoria
            });
        }
    
    }
}
