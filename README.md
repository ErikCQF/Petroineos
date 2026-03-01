# Petroineos

## Development Challenge – Requirements

This version uses an **In-Memory Queue**.  
It can easily be replaced by a durable by implementing the `Buffers/IJobQueue` interface.

---

# Workflow

| SchedulerReport (Producer) | → | IJobQueue (Queue) | → | PowerVolume (Consumer) | → | IPowerVolumeProcessor | → | IDataPublisher |
|----------------------------|---|-------------------|---|------------------------|---|-----------------------|---|----------------|
| Background Service |   | RabbitMQ / AzureBus / ... |   | Background Service |   | PowerServices |   | File Output |

---

## Processing Description

**PowerVolumeProcessor** get trades by date and consolidate them, then send data to data publisher service, which export the data to csv file.

---

# Infrastructure

```
/Buffers
/Producers
/Consumers
/PowerServices
```

---

# Windows Service Settings

## DataPublisher

```json
{
  "DataPublisher": {
    "FilePath": "C:\\Exports",
    "StartFileName": "PowerPosition_",
    "CsvSeparetor": ";"
  }
}
```

---

## Scheduler

```json
{
  "Scheduler": {
    "IntervalInMinutes": "1"
  }
}
```

---

## Resilience

```json
{
  "Resilience": {
    "MaxRetryAttempts": 5,
    "DelayMilliseconds": 2000
  }
}
```
