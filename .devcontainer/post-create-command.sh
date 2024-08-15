#/bin/sh

# needs privilege, so ignore for now.
# dotnet workload update
cd sk-llm-playground-console
echo "init dotnet user secrets"
dotnet user-secrets init

if [[ -z "$OPENAI_APIKEY" ]]
then
    echo "API KEY NOT SET"
    exit 2
fi

echo "open ai key debug $OPENAI_APIKEY"
dotnet user-secrets set "OpenAI:apiKey" $OPENAI_APIKEY