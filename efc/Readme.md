## Mapping of Entity (Records), Models and DataTransferObjects (DTO's)

:::mermaid
flowchart TD

DB["DbContext"]
RP["Repository"]
QS["QueryService"]

EF["Entity / Record"]
Model["Model"]
DTO["DTO"]
Command["Command"]
Event["Event"]

Model -.->|maps to| DTO
EF -.->|maps to| DTO
DB -->|Materializes| EF
RP -->|Injects| DB
RP -->|Materializes| Model
EF <-.->|maps to | Model
QS -->|Materializes| DTO
QS -->|Injects| DB
Model -->|Consumes| Command
Model -->|Consumes| Event
Event -->|Contains| DTO

classDef blue fill:#4a90e2,stroke:#2c5aa0,stroke-width:2px,color:#fff;
classDef red fill:#e74c3c,stroke:#c0392b,stroke-width:2px,color:#fff;
classDef green fill:#27ae60,stroke:#1e8449,stroke-width:2px,color:#fff;
classDef yellow fill:#f39c12,stroke:#d68910,stroke-width:2px,color:#fff;

class DB,RP,QS blue;
class EF red;
class Model green;
class Event,Command,DTO yellow;
:::