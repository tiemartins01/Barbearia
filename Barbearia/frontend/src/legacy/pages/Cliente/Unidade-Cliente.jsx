import {Clock, MapPin, Phone } from "lucide-react";
export default function UnidadeCliente() {

    return (
        <section className="mt-8">
            <h3 className="mb-3 font-serif text-xl">
                Nossa Unidade
            </h3>

            <article className="rounded-3xl border bg-card p-5">
                <h4 className="font-semibold text-lg">
                    BarberClub — Centro
                </h4>

                <div className="mt-4 space-y-3 text-sm text-muted-foreground">
                    <div className="flex items-center gap-3">
                        <MapPin className="h-4 w-4 text-primary" />
                        <span>Rua das Tesouras, 123</span>
                    </div>

                    <div className="flex items-center gap-3">
                        <Phone className="h-4 w-4 text-primary" />
                        <span>(11) 99999-0000</span>
                    </div>

                    <div className="flex items-center gap-3">
                        <Clock className="h-4 w-4 text-primary" />
                        <span>Seg-Sáb · 08:00–19:00</span>
                    </div>
                </div>
            </article>
        </section>
    );
}