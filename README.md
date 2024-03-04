# movie-renter
Dummie movie renter application. It could be selling/renting anything to be honest. I just needed a project to practice stuff like:

- DDD: I built this project using DDD architecture.
- Docker: a must in any project. Added docker compose for easiness to manage various (and even only one) container.
- Background services: not the first one I build from scratch, but wanted to do so in a personal project.
- SignalR: I wanted to use it in the background service to send orders to branches. Probably Service Bus is better in a real scenario, but I wanted to play around with this and it works like a charm.
- Auth: I played around with both IdentityServer (now managed by Duende) and the generic one provided by .Net. The second one is more than enough for small projects. And free.
- JWT Token: used primarily for admins. I wanted to play around with Auth and SignalR to deliver certain message to certain (and authenticated) clients.
