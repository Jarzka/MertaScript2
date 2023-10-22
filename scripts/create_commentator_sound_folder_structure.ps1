$folders = @(
    "bomb-planted-client",
    "bomb-planted-enemy",
    "defuse-client",
    "hostage-taken-enemy",
    "kill-headshot-client",
    "kill-headshot-enemy",
    "kill-headshot-juhis-client",
    "kill-headshot-machine-gun-client",
    "kill-hegrenade-client",
    "kill-hegrenade-enemy",
    "kill-inferno-client",
    "kill-inferno-enemy",
    "kill-knife-client",
    "kill-knife-enemy",
    "round-draw",
    "round-start-client-winning",
    "round-start-client-winning-massively",
    "round-start-enemy-winning",
    "round-start-enemy-winning-massively",
    "score-client",
    "score-client-1-0",
    "score-client-1-1",
    "score-client-1-7",
    "score-client-2-0",
    "score-client-2-1",
    "score-client-2-2",
    "score-client-2-3",
    "score-client-3-0",
    "score-client-3-1",
    "score-client-3-2",
    "score-client-4-0",
    "score-client-5-1",
    "score-client-6-1",
    "score-enemy",
    "score-enemy-1-0",
    "score-enemy-1-1",
    "score-enemy-2-0",
    "score-enemy-2-2",
    "score-enemy-3-1",
    "score-even-client",
    "score-win-client",
    "score-win-enemy",
    "suicide",
    "teamkiller-client",
    "teamkiller-enemy",
    "time-0-02",
    "time-0-03",
    "time-0-10",
    "time-0-15",
    "time-0-20",
    "time-0-28",
    "time-0-30",
    "time-0-40",
    "time-1-00"
)

foreach ($folder in $folders) {
    New-Item -ItemType Directory -Name $folder
}

Write-Host "Folders created successfully."
