export class TimeUtils {
  static convertTimeToSeconds(time: string): number {
    const parts = time.split(':').map(Number);
    const hours = parts[0] || 0;
    const minutes = parts[1] || 0;
    const seconds = parts[2] || 0;
    return hours * 3600 + minutes * 60 + seconds;
  }

  static formatSecondsToMMSS(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${this.padNumber(minutes)}:${this.padNumber(remainingSeconds)}`;
  }

  private static padNumber(num: number): string {
    return num < 10 ? `0${num}` : `${num}`;
  }
}
