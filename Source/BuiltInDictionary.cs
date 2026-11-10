using System;
using System.Collections.Generic;

namespace UniversalRimWorldTranslator
{
    public static class BuiltInDictionary
    {
        private static readonly Dictionary<string, string> Words = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"yes","sí"},{"no","no"},{"cancel","cancelar"},{"accept","aceptar"},{"close","cerrar"},
            {"back","atrás"},{"next","siguiente"},{"previous","anterior"},{"continue","continuar"},
            {"save","guardar"},{"load","cargar"},{"delete","eliminar"},{"rename","renombrar"},
            {"settings","configuración"},{"options","opciones"},{"controls","controles"},{"research","investigación"},
            {"work","trabajo"},{"assign","asignar"},{"animals","animales"},{"animal","animal"},
            {"food","comida"},{"medicine","medicina"},{"wood","madera"},{"steel","acero"},
            {"stone","piedra"},{"silver","plata"},{"gold","oro"},{"component","componente"},
            {"components","componentes"},{"cloth","tela"},{"leather","cuero"},{"meat","carne"},
            {"plant","planta"},{"plants","plantas"},{"tree","árbol"},{"trees","árboles"},
            {"colonist","colono"},{"colonists","colonos"},{" prisoner","prisionero"},
            {"health","salud"},{"hunger","hambre"},{"thirst","sed"},{"rest","descanso"},
            {"temperature","temperatura"},{"comfort","comodidad"},{"beauty","belleza"},
            {"room","habitación"},{"bedroom","dormitorio"},{"kitchen","cocina"},{"hospital","hospital"},
            {"storage","almacenamiento"},{"stockpile","zona de almacenamiento"},{" caravan","caravana"},
            {"attack","atacar"},{"draft","reclutar"},{"undraft","quitar reclutamiento"},
            {"build","construir"},{"deconstruct","deconstruir"},{"repair","reparar"},
            {" harvest","cosechar"},{"plant cut","cortar plantas"},{"mine","minar"},
            {"hunt","cazar"},{"cook","cocinar"},{"clean","limpiar"},{"haul","transportar"},
            {"fire","fuego"},{"snow","nieve"},{"rain","lluvia"},{"fog","niebla"},
            {"winter","invierno"},{"summer","verano"},{"spring","primavera"},{"autumn","otoño"},
            {"day","día"},{"night","noche"},{"hour","hora"},{"hours","horas"},
            {"danger","peligro"},{"warning","advertencia"},{"error","error"},{"unknown","desconocido"}
        };

        private static readonly Dictionary<string, string> Phrases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"New game","Nueva partida"},{"Load game","Cargar partida"},{"Save game","Guardar partida"},
            {"Main menu","Menú principal"},{"Back to game","Volver al juego"},
            {"Are you sure?","¿Estás seguro?"},{"Not enough food","No hay suficiente comida"},
            {"Not enough space","No hay suficiente espacio"},{"Cannot afford","No se puede pagar"},
            {"Work priorities","Prioridades de trabajo"},{"Assign to all","Asignar a todos"},
            {"Needs","Necesidades"},{"Medical operations","Operaciones médicas"},
            {"Prisoner tab","Pestaña de prisioneros"},{"Animal tab","Pestaña de animales"},
            {"Research finished","Investigación terminada"},{"Research completed","Investigación completada"}
        };

        public static bool TryTranslate(string text, out string translated)
        {
            if (Phrases.TryGetValue(text.Trim(), out translated)) return true;
            if (Words.TryGetValue(text.Trim(), out translated)) return true;
            translated = null;
            return false;
        }
    }
}