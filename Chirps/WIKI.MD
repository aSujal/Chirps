# Dokumentation - Chirps Platform

## Datenbankdiagramm

5 Haupttabellen:

### Tabellen:
- **User**: Speichert Benutzerinformationen (Username, Email, Passwort, Beschreibung)
- **Chirp**: Speichert die Kurznachrichten
- **Peep**: Speichert Themen
- **Like**: Speichert die Likes zwischen Benutzern und Chirps
- **PeepChirp**: Verknüpfungs Tabelle zwischen Chirps und Peeps

### Beziehungen:
- Ein User kann mehrere Chirps erstellen (1:N)
- Ein User kann mehrere Likes vergeben (1:N)
- Ein Chirp kann mehrere Likes erhalten (1:N)
- Ein Chirp kann mehrere Peeps enthalten (N:M)
- Ein Peep kann in mehreren Chirps vorkommen (N:M)

## Services

### UserService
ist für die Verwaltung von alle benutzerbezogenen Operationen wie Registrierung und Login zuständig.
Bietet Methoden für die Benutzerstatistiken (Anzahl Chirps, erhaltene/vergebene Likes) und den letzten Chirps eines Benutzers.

### ChirpService
Zuständig für das Erstellen und Abrufen von Chirps. 
Ermöglicht das Filtern von Chirps nach bestimmten Peeps und zeigt standardmäßig die 10 neuesten Chirps an.

### PeepService
Extrahiert Peeps aus Chirps mit dem "<" Symbol und validiert deren Länge (3-16 Zeichen). 
Bietet Methoden wie, die 5 häufigsten Peeps der letzten 24 Stunden.

### LikeService
Handelt das Like/entlike System mit Toggle Funktionalität und verhindert doppelte Likes. 
Bietet Methoden zum Überprüfen des Like Status und zum Abrufen der Gesamtanzahl von Likes pro Chirp.