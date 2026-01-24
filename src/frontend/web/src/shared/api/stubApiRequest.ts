const sleep = (ms: number) => new Promise<void>((r) => setTimeout(r, ms));

export type DemoOpt = { delay?: number, forceError: string | null };

export async function demoFunction({ delay, forceError }: DemoOpt): Promise<void> {
    if (delay !== undefined) await sleep(delay);
    if (forceError) throw new Error(forceError);
}
