interface MatchEvent {
  typeEvenement: string;
  joueur?: { id: number };
  joueurVictime?: { id: number };
  equipe?: { id: number; nom: string };
  points?: number;
  temps?: string;
  quartTemps?: number;
  typeFaute?: string;
  joueurSortantId?: number;
  joueurEntrantId?: number;
}

export default MatchEvent;
