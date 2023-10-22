$folders = @(
    "Defused-Bomb",
    "Die",
    "Die-He",
    "Die-Headshot",
    "Die-Knife",
    "Die-Molotov",
    "Kill",
    "Kill-He",
    "Kill-Headshot",
    "Kill-Knife",
    "Kill-Molotov",
    "Rescued-Hostage",
    "Start",
    "Suicide",
    "Throw"
)

foreach ($folder in $folders) {
    New-Item -ItemType Directory -Name $folder
}

Write-Host "Folders created successfully."
