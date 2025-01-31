using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevEnterprise.Scrapi.Sdk.BrowserCommands;

/// <summary>
/// Handles serializing and deserializing browser command JSON.
/// </summary>
internal sealed class BrowserCommandJsonConverter : JsonConverter
{
  /// <inheritdoc/>
  public override bool CanConvert(Type objectType) => throw new NotImplementedException();

  /// <inheritdoc/>
  public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
  {
    var commands = existingValue as List<IBrowserCommand>;

    if (commands is not null && reader.TokenType == JsonToken.StartArray)
    {
      while (reader.Read())
      {
        if (reader.TokenType == JsonToken.EndArray)
        {
          break;
        }

        if (reader.TokenType == JsonToken.StartObject)
        {
          while (reader.Read())
          {
            if (reader.TokenType == JsonToken.EndObject)
            {
              break;
            }

            if (reader.TokenType == JsonToken.PropertyName)
            {
              switch (reader.Value!.ToString()!.ToLowerInvariant())
              {
                case "click":
                case "tap":
                  while (reader.Read())
                  {
                    if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.EndObject)
                    {
                      break;
                    }
                  }

                  var clickCommand = new ClickCommand { TargetSelector = reader.Value?.ToString() ?? string.Empty };
                  commands.Add(clickCommand);
                  break;

                case "wait":
                  while (reader.Read())
                  {
                    if (reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.EndObject)
                    {
                      break;
                    }
                  }

                  var waitCommand = new WaitCommand { Milliseconds = Convert.ToInt32(reader.Value ?? 0) };
                  commands.Add(waitCommand);
                  break;

                case "waitfor":
                case "wait_for":
                case "wait-for":
                  while (reader.Read())
                  {
                    if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.EndObject)
                    {
                      break;
                    }
                  }

                  var waitForCommand = new WaitForCommand { TargetSelector = reader.Value?.ToString() ?? string.Empty };
                  commands.Add(waitForCommand);
                  break;

                case "javascript":
                case "js":
                case "eval":
                case "evaluate":
                  while (reader.Read())
                  {
                    if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.EndObject)
                    {
                      break;
                    }
                  }

                  var javaScriptCommand = new JavaScriptCommand { Script = reader.Value?.ToString() ?? string.Empty };
                  commands.Add(javaScriptCommand);
                  break;

                case "input":
                case "fill":
                case "type":
                  while (reader.Read())
                  {
                    if (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.EndObject)
                    {
                      break;
                    }
                  }

                  if (reader.TokenType == JsonToken.StartObject)
                  {
                    var inputCommand = new InputCommand();
                    while (reader.Read())
                    {
                      if (reader.TokenType == JsonToken.PropertyName)
                      {
                        inputCommand.TargetSelector = reader.Value?.ToString() ?? string.Empty;
                      }
                      else if (reader.TokenType == JsonToken.String)
                      {
                        inputCommand.InputValue = reader.Value?.ToString() ?? string.Empty;
                      }
                      else if (reader.TokenType == JsonToken.EndObject)
                      {
                        break;
                      }
                    }

                    commands.Add(inputCommand);
                  }

                  break;

                case "select":
                case "choose":
                case "pick":
                  while (reader.Read())
                  {
                    if (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.EndObject)
                    {
                      break;
                    }
                  }

                  if (reader.TokenType == JsonToken.StartObject)
                  {
                    var inputCommand = new SelectCommand();
                    while (reader.Read())
                    {
                      if (reader.TokenType == JsonToken.PropertyName)
                      {
                        inputCommand.TargetSelector = reader.Value?.ToString() ?? string.Empty;
                      }
                      else if (reader.TokenType == JsonToken.String)
                      {
                        inputCommand.SelectValue = reader.Value?.ToString() ?? string.Empty;
                      }
                      else if (reader.TokenType == JsonToken.EndObject)
                      {
                        break;
                      }
                    }

                    commands.Add(inputCommand);
                  }

                  break;
              }
            }
          }
        }
      }
    }

    return commands;
  }

  /// <inheritdoc/>
  public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
  {
    if (value is not null)
    {
      writer.WriteStartArray();

      foreach (var command in JToken.FromObject(value))
      {
        var key = command["CommandName"]!.ToString().ToLowerInvariant();

        writer.WriteStartObject();
        writer.WritePropertyName(key);

        switch (key)
        {
          case "click":
          case "tap":
            writer.WriteValue(command["TargetSelector"]);
            break;

          case "wait":
            writer.WriteValue(command["Milliseconds"]);
            break;

          case "waitfor":
          case "wait_for":
          case "wait-for":
            writer.WriteValue(command["TargetSelector"]);
            break;

          case "javascript":
          case "js":
          case "eval":
          case "evaluate":
            writer.WriteValue(command["Script"]);
            break;

          case "input":
          case "fill":
          case "type":
            if (command["TargetSelector"] is not null)
            {
              writer.WriteStartObject();
              writer.WritePropertyName(command["TargetSelector"]!.ToString());
              writer.WriteValue(command["InputValue"]);
              writer.WriteEndObject();
            }

            break;

          case "select":
          case "choose":
          case "pick":
            if (command["TargetSelector"] is not null)
            {
              writer.WriteStartObject();
              writer.WritePropertyName(command["TargetSelector"]!.ToString());
              writer.WriteValue(command["SelectValue"]);
              writer.WriteEndObject();
            }

            break;
        }

        writer.WriteEndObject();
      }

      writer.WriteEndArray();
    }
  }
}
