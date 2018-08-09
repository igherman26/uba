# UBA - User Behaviour Analytics

## Project information

This project is my college thesis. 
It represents a system used to monitor a user's behaviour and detect anomalies for Windows.

It is used to monitor:
- OS usage - used processes and if they're elevated or not, access/deletion/modification of sensitive files and access/deletion/modification of registry keys
- resource usage  - CPU, GPU, Memory and Disk usage
- network usage - protocols used, accessed domains, the countries with which the user communicates and the volume of data received and sent

The project represents a system for monitorization and anomaly detections and it is not supposed to stop attacks/infections but only to alert the user that his behaviour is unusual.

## Components

The projects is made of the following 5 components:
![image](https://user-images.githubusercontent.com/40753750/43878170-6af69352-9ba6-11e8-82a9-73df2ae02beb.png)

### Dashboard / UI
  Represents the project's user interface. It's role is to load the Manager component and offer its functionalities to the user.
### Manager
  It's roles are the following:
    - manage user profiles and their data
    - load data fetching modules
    - process the data obtained by the data fetching modules
    - monitor behaviour and generate alerts
### Events Data Fetcher
Obtains data related to **OS usage** using **Windows Events Logs**.
### Network Data Fetcher
Obtains data related to **network usage** using **PcapDotNet**, **netstat** and **http://ip-api.com**.
### Performance Data Fetcher
Obtains data related to **resource usage** using **Performance Counters** and ***NVAPI**.

## Other info
### Used in this project:
- Windows Events Logs, Performance Counters and netstat(code from https://gist.github.com/cheynewallace/5971686)
- PcapDotNet
- http://ip-api.com API
- NVAPI (code from https://github.com/coraxx/CpuGpuGraph/)
- Newtonsoft.Json
- application icon from https://icons8.com/icon/set/detective/dusk

### The project's documentation is available in docs folder but it is written in romanian.

## Screenshots
![image](https://user-images.githubusercontent.com/40753750/43878894-1fe7286e-9baa-11e8-8204-77f855016f9b.png)
![image](https://user-images.githubusercontent.com/40753750/43878904-28647ef6-9baa-11e8-923b-1b5786f45251.png)
![image](https://user-images.githubusercontent.com/40753750/43878797-9d58e77a-9ba9-11e8-9e8d-142896504c7a.png)
![image](https://user-images.githubusercontent.com/40753750/43878803-a72681ea-9ba9-11e8-8389-08f6b9c116ce.png)
![image](https://user-images.githubusercontent.com/40753750/43878806-b03bd2a8-9ba9-11e8-83f7-97bd8163c538.png)
