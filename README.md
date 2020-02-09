# FreeEDR
Senior Project 2019-2020 CCI

### 1.What does the project do?


###### Overview:
This project is an open-source Endpoint Detection and Response system that is designed for low budget or minimal security resource organizations. Organizations that don’t have a significant security budget can find it difficult to include workstations in
their monitoring scope. Forwarding logs from all your workstations can be expensive because most
SIEMs are priced based on log ingestion and tools such as EDR are just as expensive. This project aims to
setup a series of scripts which will allow organizations who don’t have the ability to purchase or
implement an enterprise solution. This solution is aimed at devices running Windows as it relies on
querying Windows events using PowerShell.

###### Sigma:
Sigma is a generic and open signature format that allows you to describe relevant log events in a
straightforward manner. This means you can write a rule is Sigma format and it can be translated, using
a tool called SigmaC, to the language of your preference. Sigma will be used to write detection rules for
endpoints for this project. You can read more about Simga on their GitHub page.
https://github.com/Neo23x0/sigma.

###### Rule Repository:
The first aspect of this project is a rule repository where you can write Sigma rules to detect on process
or network events on your endpoints. The Sigma rules will be translated to PowerShell rules on the rule
repository server. Every day endpoints will reach out to the rule repository server to request any new
PowerShell rules.

###### Endpoints:
On each endpoint there will be a script which reaches out to the rule repository and request any new
rules. The rules will then be run to see if there is a matching Windows event. There are two types of
rules: process detection rules and network detection rules.

###### Forensics:
If the rule matches on a process event, then the script will gather forensics information about the
specific process. If the rule matches on a network event, the script will gather forensics information
about the network connection.

###### Custom Windows Event:
The information gathered in the forensics phase will be written into a custom windows event. This
event will contain all the information that you would want about the process or network connection.
Now organizations can setup this one event to be forwarded to their SIEM solution, or event setup email
or message alerts when these events fire.

---
### 2. Why is the project useful?

Organizations that don’t have a significant security budget can find it difficult to include workstations in their monitoring scope. Forwarding logs from all your workstations can be expensive because most SIEMs (Security Information and Event Management) are priced based on log ingestion and tools such as EDR (Endpoint Detection & Response) are just as expensive. This project aims to setup a series of scripts which will allow organizations who don’t have the ability to purchase or implement an enterprise solution the functionality of logging for their endpoints. This solution is aimed at devices running Windows as it relies on querying Windows events using power.

The first aspect of this project is a rule repository where you can write Sigma rules to detect on process or network events on your endpoints. The Sigma rules will be translated to PowerShell rules on the rule repository server. Every day endpoints will reach out to the rule repository server to request any new PowerShell rules. On each endpoint there will be a script which reaches out to the rule repository and request any new rules. The rules will then be run to see if there is a matching Windows event. There are two types of rules: process detection rules and network detection rules. If the rule matches on a process event, then the script will gather forensics information about the specific process. If the rule matches on a network event, the script will gather forensics information about the network connection.

The information gathered in the forensics phase will be written into a custom windows event. This event will contain all the information that you would want about the process or network connection. Now organizations can setup this one event to be forwarded to their SIEM solution, or event setup email or message alerts when these events fire.

---

### 3. How can users get started with the project?
#### Dashboard

###### Setup

npm run build - bundles the project into one JS file for deployment

npm run start - starts the project in webpack dev server on localhost:3333


###### Technologies

* React UI

#### Infra
---
### 4. Where can users get help with the project?
---
### 5. Who maintains and contributes to the project?
Currently, the project is being developed and maintained by a senior design project team, FreeEDR, of the College of Computing and Informatics of Drexel University. The seven team members include students from the College of Computing and Informatics and the College of Engineering. 
