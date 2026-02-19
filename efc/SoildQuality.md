# Soil Quality subdomein

Het Soil Quality of grond kwaliteit subdomein houd o.b.v. metingen van de kwaliteit van de grond in de gaten. BulbQualityReports, Floweringtrials of externe grond metingen zijn de basis voor een visueel wereldwijd inzicht van de kwaliteit over de seizoenen heen.

De natuurlijke identificatie van grond is een gps coordinaten gebied.

[picture of earth with markers]

[(_TOC_)]

# Domain Model

De grond kwaliteit bevat informatie over  ziekten van een specifiek seizoen in een specifiek gebied gedefinieerd door gps-coördinaten. De informatie over ziekten wordt weergegeven in percentages van besmette grond en een indicator of deze toeneemt op basis van historische data.

Grond metingen hebben een referentie en kunnen worden gearchiveerd.

Grond kwaliteiten kunnen op planeet- of veldschaal worden gemaakt en kunnen dus overlappen.

## Model
:::mermaid
classDiagram
    class SoilMeasurement {
        - GpsArea Area
        - DateTime MeasuredOnUtc
        - Percentage FusariumPercentage
        - Percentage MildewPercentage
        - bool IsArchived
        - string ReportReference
    }

    class SoilQuality {
        - GpsArea Area
        - Season Season
        - List~SoilMeasurement~ SoilMeasurements
        - Percentage AverageFusariumPercentage
        - Percentage AverageMildewPercentage
        - SoilQualityChange AverageFusariumPercentageChange
        - SoilQualityChange AverageMildewPercentageChange
        Soil.Create(contract)
        Apply(contract)
    }

    SoilQuality "*" o-- "*" SoilMeasurement : has
:::


## Contracts

Events en Commands voorzien het model van data. Een event of command kan data voor meerdere SoilQuality modellen bevatten en allen worden idempotent bijgewerkt.

:::mermaid
classDiagram

    namespace Subscribes {
        class BulbQualityReportCreatedEvent {
            - NaturalIdentifier Identifier
            - GpsArea GpsArea
            - DateTime MeasuredOnUtc
            - Percentage FusariumPercentage
            - Percentage MildewPercentage
        }

        class FloweringTrialReportCreatedEvent {
            - NaturalIdentifier Identifier
            - GpsArea GpsArea
            - DateTime MeasuredOnUtc
            - Percentage FusariumPercentage
            - Percentage MildewPercentage
        }

        class ImportSoilMeasurementsCommand {
            - NaturalIdentifier Identifier
            - List~ImportSoilMeasurement~ SoilMeasurements
        }

        class ImportSoilMeasurement {
            - GpsArea GpsArea
            - DateTime MeasuredOnUtc
            - Percentage FusariumPercentage
            - Percentage MildewPercentage
        }
    }

    ImportSoilMeasurementsCommand "1" o-- "*" ImportSoilMeasurement : has

    namespace Publishes {
        class SoilQualityUpdatedEvent {
            - SoilQualityDTO Data
        }

        class SoilQualityWarningEvent {
            - SoilQualityDTO Data
        }
    }

    namespace DTO {
        class SoilQualityDTO {
            - GpsArea GpsArea
            - Season Season
            - Percentage AverageFusariumPercentage
            - SoilQualityChange AverageFusariumPercentageChange
        }
    }

    SoilQualityUpdatedEvent "1" o-- "1" SoilQualityDTO : has
    SoilQualityWarningEvent "1" o-- "1" SoilQualityDTO : has
:::

# QueryService

Informatie kan worden opgevraagd via de queryservice.

:::mermaid
classDiagram

    class SoilQualityQueryService {
        SoilQualityDTO GetAllBy(GpsArea GpsArea, Season? Season)
    }
:::