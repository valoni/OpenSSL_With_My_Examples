# OpenSSL_With_My_Examples
OpenSSL With Example

How to Genearate SSL Certificate 

------------------------------------
GENERATION OF PRIVATE KEYS 
------------------------------------
openssl req -newkey rsa:2048 -keyform PEM -keyout ca.key -x509 -days 3650 -outform PEM -out ca.cer


------------------------------------
SERVER / CERT GENERATION
------------------------------------
openssl genrsa -out server.key 2048

openssl req -new -key server.key -out server.req -sha256

openssl x509 -req -in server.req -CA ca.cer -CAkey ca.key -set_serial 100 -extensions server -days 1460 -outform PEM -out server.cer -sha256

openssl pkcs12 -export -inkey server.key -in server.cer -out server.pfx


------------------------------------
CLIENT / CERT GENERATION
------------------------------------
openssl genrsa -out client.key 2048

openssl req -new -key client.key -out client.req

openssl x509 -req -in client.req -CA ca.cer -CAkey ca.key -set_serial 101 -extensions client -days 365 -outform PEM -out client.cer

openssl pkcs12 -export -inkey client.key -in client.cer -out client.pfx



How To Use Generated Certificates in C# Application (DEMO / SERVER with CLIENT using TLS and Certificates during communication)
(there's two application SERVER and CLIENT with source code in C#)
